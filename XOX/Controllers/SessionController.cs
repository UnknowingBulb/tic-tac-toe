using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XOX.BLObjects;
using Newtonsoft.Json;
using XOX.Enums;
using System;
using Microsoft.AspNetCore.Http;

namespace XOX.Controllers
{
    [ApiController]
    [Route("session")]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;

        public SessionController(ILogger<SessionController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(int sessionId)
        {
            Session session = SessionListHandler.GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");
            return Ok(JsonConvert.SerializeObject(session));
        }

        [HttpPost, Route("/start")]
        public IActionResult StartSession()
        {
            Guid userId = setAuth();
            User user = UserListHandler.AddUser(new User(userId, true));
            Session session = new Session(user);
            SessionListHandler.AddSession(session);
            return Ok(JsonConvert.SerializeObject(session));
        }

        [HttpPost, Route("/connect")]
        public IActionResult Connect(int sessionId)
        {
            Session session = SessionListHandler.GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");

            Guid userId = setAuth();
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
            return Ok(JsonConvert.SerializeObject(session));
        }

        [HttpPost, Route("/setMark")]
        public IActionResult SetMark(int sessionId, int x, int y)
        {
            Session session = SessionListHandler.GetSession(sessionId);;
            if (session == null)
                return NotFound("Игровая сессия не найдена");
            if (session.State == SessionState.Finished || session.State == SessionState.Undefined)
                return BadRequest("Игровая сессия завершена или не найдена");

            Guid userId = setAuth();
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
            return Ok(JsonConvert.SerializeObject(session));
        }

        [HttpPost, Route("/finish")]
        public IActionResult FinishSession(int sessionId)
        {
            SessionListHandler.Remove(sessionId);
            return Ok();
        }

        private Guid getAuth()
        {
            var context = HttpContext;
            string userId = context.Request.Cookies["user-id"];

            if (userId == null)
                return Guid.Empty;
            return Guid.Parse(userId);
        }

        private Guid setAuth()
        {
            Guid userId = getAuth();
            if (userId != Guid.Empty)
                return userId;

            userId = Guid.NewGuid();
            // Method 2 - Add to current context
            var context = HttpContext;
            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(20),
                IsEssential = true,
                HttpOnly = false,
                Secure = false,
            };

            context.Response.Cookies.Append("user-id", userId.ToString(), cookieOptions);
            return userId;
        }
    }
}
