using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XOX.Objects;
using Newtonsoft.Json;
using XOX.Enums;

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
            var session = new Session();
            SessionListHandler.AddSession(session);
            return Ok(JsonConvert.SerializeObject(session));
        }

        [HttpPost, Route("/connect")]
        public IActionResult Connect(int sessionId)
        {
            Session session = SessionListHandler.GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");
            //Если свободных слотов нет
            if (!((session.Player1 == null || session.Player2 == null) || 

                //TODO: Заменить 1/2 на пользователя
                (session.Player1.Id != 1 || session.Player2.Id != 2)))
                return NotFound("Нет свободных слотов");

            session.Player2.Id = 2;
            SessionListHandler.AddSession(session);
            return Ok(JsonConvert.SerializeObject(session));
        }

        [HttpPost, Route("/setMark")]
        public IActionResult SetMark(int sessionId, int x, int y)
        {
            Session session = SessionListHandler.GetSession(sessionId);
            if (session == null)
                return NotFound("Игровая сессия не найдена");
            if (session.State == SessionState.Finished || session.State == SessionState.Undefined)
                return ValidationProblem("Игровая сессия завершена или не найдена");
            //Если свободных слотов нет
            if (!((session.Player1 == null || session.Player2 == null) ||

                //TODO: Заменить 1/2 на пользователя
                (session.Player1.Id != 1 || session.Player2.Id != 2)))
                return Unauthorized("Вы не участвуете в игре, можно только смотреть");

            User user = UserListHandler.GetUser(1);

            if (user.Active == false)
                return Forbid("Действие запрещено, не ваш ход");
            session.Field.Cells[x, y].Value = user.Mark;
            user.Active = false;
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

    }
}
