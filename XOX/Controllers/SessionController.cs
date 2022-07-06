using Microsoft.AspNetCore.Mvc;
using XOX.BLObjects;
using Newtonsoft.Json;
using XOX.Enums;
using System;
using XOX.Services;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;

namespace XOX.Controllers
{
    public class SessionController : Controller
    {
        private INotificationsService _notificationsService;
        private IClientService _clientService;
        private IServerSentEventsClientIdProvider _cookies;

        public SessionController(INotificationsService notificationsService, IClientService clientService, IServerSentEventsClientIdProvider cookies)
        {
            _notificationsService = notificationsService;
            _clientService = clientService;
            _cookies = cookies;
        }

        [HttpGet, Route("/getSession")]
        public IActionResult GetSession(int sessionId)
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            Session session = SessionListHandler.GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");
            SessionDto responseData = new SessionDto(session, userId);
            return Ok(JsonConvert.SerializeObject(responseData));
        }

        [ActionName("session-reciever")]
        [AcceptVerbs("GET")]
        public IActionResult Get()
        {
            return LocalRedirect("/fetch-data");
        }

        [HttpPost, Route("/start")]
        public IActionResult StartSession()
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.AddUser(new User(userId, true));
            Session session = new Session(user);
            SessionListHandler.AddSession(session);
            _clientService.AddUserToGroup(userId, $"session{session.Id}");
            SessionDto responseData = new SessionDto(session, userId);
            return Ok(JsonConvert.SerializeObject(responseData));
        }

        [HttpPost, Route("/connect")]
        public IActionResult Connect(int sessionId)
        {
            Session session = SessionListHandler.GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");

            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.GetUser(userId);
            if (user == null || (session.Player1Id != null && session.Player1Id != userId))
                user = UserListHandler.AddUser(new User(userId, false));
            //If no empty slots
            if (!((session.Player1Id == Guid.Empty || session.Player2Id == Guid.Empty) ||
                (session.Player1Id == userId || session.Player2Id == userId)))
                return NotFound("Нет свободных слотов");

            if (session.Player1Id == Guid.Empty || session.Player1Id == userId)
                session.Player1Id = user.Id;
            else
                session.Player2Id = user.Id;
            SessionListHandler.AddSession(session);
            _clientService.AddUserToGroup(userId, $"session{session.Id}");
            SessionDto responseData = new SessionDto(session, userId);
            return Ok(JsonConvert.SerializeObject(responseData));
        }

        [HttpPost, Route("/setMark")]
        public async Task<IActionResult> SetMark(int sessionId, int x, int y)
        {
            Session session = SessionListHandler.GetSession(sessionId);
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

            User user = UserListHandler.GetUser(userId);

            if (session.Field.Cells[x, y].Value != string.Empty)
                return BadRequest("Ячейка занята, попробуйте другую");
            session.Field.Cells[x, y].Value = user.Mark;
            session.IsActivePlayer1 = !session.IsActivePlayer1;
            if (session.Field.IsGameCompleted())
                session.State = SessionState.Finished;
            else
                session.State = SessionState.InProgress;
            SessionListHandler.AddSession(session);

            string responseDataJson = JsonConvert.SerializeObject(new SessionDto(session, userId));
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
