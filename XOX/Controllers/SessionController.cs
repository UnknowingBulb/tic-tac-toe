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
        public IActionResult Get3(int sessionId)
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.AddUser(new User(userId, true));
            Session session = SessionListHandler.GetSession(sessionId);
           if (session == null)
                session = new Session(user);
            return Ok(JsonConvert.SerializeObject(session));
        }

        [ActionName("session-reciever")]
        [AcceptVerbs("GET")]
        public IActionResult Get2()
        {
            return View("Receiver");
        }

        [HttpPost, Route("/start")]
        public IActionResult StartSession()
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.AddUser(new User(userId, true));
            Session session = new Session(user);
            SessionListHandler.AddSession(session);
            //_clientService.AddUserToGroup(userId, $"session{session.Id}");
            return Ok(JsonConvert.SerializeObject(session));
        }

        [HttpPost, Route("/connect")]
        public IActionResult Connect(int sessionId)
        {
            Session session = SessionListHandler.GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");

            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.GetUser(userId);
            if (user == null)
                user = UserListHandler.AddUser(new User(userId, false));
            //Если свободных слотов нет
            if (!((session.Player1 == null || session.Player2 == null) ||
                (session.Player1.Id == userId || session.Player2.Id == userId)))
                return NotFound("Нет свободных слотов");

            if (session.Player1 == null || session.Player1.Id == userId)
                session.Player1 = user;
            else
                session.Player2 = user;
            SessionListHandler.AddSession(session);
            //_clientService.AddUserToGroup(userId, $"session{session.Id}");
            return Ok(JsonConvert.SerializeObject(session));
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
            if (session.Player1.Id == userId && session.Player2 == null)
                return BadRequest("2й игрок не подключился, невозможно начать");
            //Если свободных слотов нет
            if (!((session.Player1 == null || session.Player2 == null) ||
                (session.Player1.Id == userId || session.Player2.Id == userId)))
                return Unauthorized("Вы не участвуете в игре, можно только смотреть");

            User user = UserListHandler.GetUser(userId);

            if ((session.IsActivePlayer1 && session.Player1.Id != userId)||
                (!session.IsActivePlayer1 && session.Player2.Id != userId ))
                return BadRequest("Действие запрещено, не ваш ход");
            session.Field.Cells[x, y].Value = user.Mark;
            session.IsActivePlayer1 = !session.IsActivePlayer1;
            if (session.Field.IsGameCompleted())
                session.State = SessionState.Finished;
            else
                session.State = SessionState.InProgress;
            SessionListHandler.AddSession(session);

            string sessionJson = JsonConvert.SerializeObject(session);
            await _notificationsService.SendNotificationAsync(sessionJson, $"session{sessionId}");
            return Ok(sessionJson);
        }

        [HttpPost, Route("/finish")]
        public IActionResult FinishSession(int sessionId)
        {
            SessionListHandler.Remove(sessionId);
            return Ok();
        }
    }
}
