using Microsoft.AspNetCore.Mvc;
using XOX.BLObjects;
using Newtonsoft.Json;
using XOX.Enums;
using System;
using XOX.Services;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;
using XOX.Database;

namespace XOX.Controllers
{
    public class SessionController : Controller
    {
        private INotificationsService _notificationsService;
        private IClientService _clientService;
        private IServerSentEventsClientIdProvider _cookies;
        private SessionContext _context;

        public SessionController(INotificationsService notificationsService, IClientService clientService, IServerSentEventsClientIdProvider cookies, SessionContext context)
        {
            _notificationsService = notificationsService;
            _clientService = clientService;
            _cookies = cookies;
            _context = context;
        }

        [HttpGet, Route("/getSession")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            _cookies.AcquireClientId(HttpContext);
            Session session = await (new SessionListHandlerDb(_context)).GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");
            SessionDto responseData = new SessionDto(session);
            return Ok(JsonConvert.SerializeObject(responseData));
        }

        [ActionName("session-reciever")]
        [AcceptVerbs("GET")]
        public IActionResult Get()
        {
            return LocalRedirect("/fetch-data");
        }

        [HttpPost, Route("/start")]
        public async Task<IActionResult> StartSession()
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.GetUser(userId);
            if (user == null)
                user = UserListHandler.AddUser(new User(userId));
            Session session = new Session(user);
            session = await (new SessionListHandlerDb(_context)).AddSession(session);
            _clientService.AddUserToGroup(userId, $"session{session.Id}");
            SessionDto responseData = new SessionDto(session);
            return Ok(JsonConvert.SerializeObject(responseData));
        }

        [HttpPost, Route("/connect")]
        public async Task<IActionResult> Connect(int sessionId)
        {
            Session session = await (new SessionListHandlerDb(_context)).GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");

            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.GetUser(userId);
            if (user == null)
                user = UserListHandler.AddUser(new User(userId));
            //If no empty slots
            if (!((session.Player1Id == Guid.Empty || session.Player2Id == Guid.Empty) ||
                (session.Player1Id == userId || session.Player2Id == userId)))
                return NotFound("Нет свободных слотов");

            if (session.Player1Id == Guid.Empty && session.Player2Id != user.Id)
            {
                if (UserListHandler.GetUser(session.Player2Id).Mark == user.Mark)
                {
                    return BadRequest("Совпадает метка с уже участвующим игроком. Измените свою метку и попробуйте снова");
                }
                session.Player1Id = user.Id;
            }
            else if (session.Player2Id == Guid.Empty && session.Player1Id != user.Id)
            {
                if (UserListHandler.GetUser(session.Player1Id).Mark == user.Mark)
                {
                    return BadRequest("Совпадает метка с уже участвующим игроком. Измените свою метку и попробуйте снова");
                }
                session.Player2Id = user.Id;
            }
            session = await (new SessionListHandlerDb(_context)).AddSession(session);
            _clientService.AddUserToGroup(userId, $"session{session.Id}");
            string responseDataJson = JsonConvert.SerializeObject(new SessionDto(session));
            await _notificationsService.SendNotificationAsync(responseDataJson, $"session{sessionId}");
            return Ok(responseDataJson);
        }

        [HttpPost, Route("/setMark")]
        public async Task<IActionResult> SetMark(int sessionId, int x, int y)
        {
            Session session = await (new SessionListHandlerDb(_context)).GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");
            if (session.State == SessionState.Finished || session.State == SessionState.Undefined)
                return BadRequest("Игровая сессия завершена или не найдена");

            Guid userId = _cookies.AcquireClientId(HttpContext);
            if (session.Player1Id == userId && session.Player2Id == Guid.Empty)
                return BadRequest("2й игрок не подключился, невозможно начать");
            //If no empty slots
            if (!((session.Player1Id == Guid.Empty || session.Player2Id == Guid.Empty) ||
                (session.Player1Id == userId || session.Player2Id == userId)))
                return Unauthorized("Вы не участвуете в игре, можно только смотреть");

            if ((session.IsActivePlayer1 && session.Player1Id != userId) ||
                (!session.IsActivePlayer1 && session.Player2Id != userId))
                return BadRequest("Действие запрещено, не ваш ход");
            if (session.Field.Cells[x, y].Value != string.Empty)
                return BadRequest("Ячейка занята, попробуйте другую");

            User user = UserListHandler.GetUser(userId);
            session.Field.Cells[x, y].Value = user.Mark;
            session.IsActivePlayer1 = !session.IsActivePlayer1;
            if (session.Field.IsGameCompleted())
                session.State = SessionState.Finished;
            else
                session.State = SessionState.InProgress;
            session = await (new SessionListHandlerDb(_context)).AddSession(session);

            string responseDataJson = JsonConvert.SerializeObject(new SessionDto(session));
            await _notificationsService.SendNotificationAsync(responseDataJson, $"session{sessionId}");
            return Ok(responseDataJson);
        }

        [HttpPost, Route("/finish")]
        public IActionResult FinishSession(int sessionId)
        {
            SessionListHandler.Remove(sessionId);
            return Ok();
        }
    }
}
