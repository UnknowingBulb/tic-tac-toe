using Microsoft.AspNetCore.Mvc;
using XOX.BLObjects;
using Newtonsoft.Json;
using XOX.Enums;
using System;
using XOX.Services;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;
using XOX.Database;
using FluentResults;

namespace XOX.Controllers
{
    [Route("session")]
    public class SessionController : Controller
    {
        private INotificationsService _notificationsService;
        private IClientService _clientService;
        private IServerSentEventsClientIdProvider _cookies;
        private SessionContext _context;
        private UserListHandlerDb _userListHandler;

        public SessionController(INotificationsService notificationsService, IClientService clientService, IServerSentEventsClientIdProvider cookies, SessionContext context)
        {
            _notificationsService = notificationsService;
            _clientService = clientService;
            _cookies = cookies;
            _context = context;
            _userListHandler = new UserListHandlerDb(_context);
        }

        [HttpGet, Route("get")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            _cookies.AcquireClientId(HttpContext);
            var sessionResult = await getSession(sessionId);
            if (sessionResult.IsFailed)
                return BadRequest(sessionResult.Errors.ToString());
            var session = sessionResult.Value;
            var player1 = await _userListHandler.GetUser(session.Player1Id);
            var player2 = await _userListHandler.GetUser(session.Player2Id);
            var responseData = new SessionDto(session, player1, player2);
            return Ok(JsonConvert.SerializeObject(responseData));
        }

        [ActionName("session-reciever")]
        [AcceptVerbs("GET")]
        public IActionResult Get()
        {
            return RedirectPermanent("/");
        }

        [HttpPost, Route("start")]
        public async Task<IActionResult> StartSession()
        {
            var userId = _cookies.AcquireClientId(HttpContext);
            var user = await _userListHandler.GetUser(userId);
            if (user == null)
                user = await _userListHandler.AddUser(new User(userId));
            var session = new Session(user);
            session = await (new SessionListHandlerDb(_context)).AddSession(session);
            _clientService.AddUserToGroup(userId, $"session{session.Id}");
            var responseData = new SessionDto(session, user, null);
            return Ok(JsonConvert.SerializeObject(responseData));
        }

        [HttpPost, Route("connect")]
        public async Task<IActionResult> Connect(int sessionId)
        {
            var sessionResult = await getSession(sessionId);
            if (sessionResult.IsFailed)
                return BadRequest(sessionResult.Errors.ToString());
            var session = sessionResult.Value;

            var userId = _cookies.AcquireClientId(HttpContext);
            var user = await _userListHandler.GetUser(userId);
            if (user == null)
                user = await _userListHandler.AddUser(new User(userId));
            //If no empty slots
            if (!((session.Player1Id == Guid.Empty || session.Player2Id == Guid.Empty) ||
                (session.Player1Id == userId || session.Player2Id == userId)))
                return NotFound("Нет свободных слотов");

            var player1 = await _userListHandler.GetUser(session.Player1Id);
            var player2 = await _userListHandler.GetUser(session.Player2Id);

            if (session.Player1Id == Guid.Empty && session.Player2Id != user.Id)
            {
                if (player2.Mark == user.Mark)
                {
                    return BadRequest("Совпадает метка с уже участвующим игроком. Измените свою метку и попробуйте снова");
                }
                session.Player1Id = user.Id;
            }
            else if (session.Player2Id == Guid.Empty && session.Player1Id != user.Id)
            {
                if (player1.Mark == user.Mark)
                {
                    return BadRequest("Совпадает метка с уже участвующим игроком. Измените свою метку и попробуйте снова");
                }
                session.Player2Id = user.Id;
                player2 = user;
            }
            session = await (new SessionListHandlerDb(_context)).AddSession(session);
            _clientService.AddUserToGroup(userId, $"session{session.Id}");
            string responseDataJson = JsonConvert.SerializeObject(new SessionDto(session, player1, player2));
            await _notificationsService.SendNotificationAsync(responseDataJson, $"session{sessionId}");
            return Ok(responseDataJson);
        }

        [HttpPost, Route("setMark")]
        public async Task<IActionResult> SetMark(int sessionId, int x, int y)
        {
            var sessionResult = await getSession(sessionId);
            if (sessionResult.IsFailed)
                return BadRequest(sessionResult.Errors.ToString());
            var session = sessionResult.Value;
            if (session.State != SessionState.InProgress && session.State != SessionState.NotStarted)
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

            var user = await _userListHandler.GetUser(userId);
            session.Field.Cells[x, y].Value = user.Mark;
            if (session.Field.IsGameFinishedWithVictory())
                session.State = SessionState.Finished;
            else if (session.Field.HasNoMoreTurns())
                session.State = SessionState.Draw;
            else
            {
                session.State = SessionState.InProgress;
                session.IsActivePlayer1 = !session.IsActivePlayer1;
            }
            session = await (new SessionListHandlerDb(_context)).AddSession(session);

            var player1 = await _userListHandler.GetUser(session.Player1Id);
            var player2 = await _userListHandler.GetUser(session.Player2Id);
            string responseDataJson = JsonConvert.SerializeObject(new SessionDto(session, player1, player2));
            await _notificationsService.SendNotificationAsync(responseDataJson, $"session{sessionId}");
            return Ok(responseDataJson);
        }

        [HttpPost, Route("retreat")]
        public async Task<IActionResult> FinishSession(int sessionId)
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            var sessionResult = await getSession(sessionId);
            if (sessionResult.IsFailed)
                return BadRequest(sessionResult.Errors.ToString());
            var session = sessionResult.Value;

            if (userId != session.Player1Id && userId != session.Player2Id)
                return Unauthorized("Вы не участвуете в игре, можно только смотреть");
            session.State = SessionState.Finished;
            session.IsActivePlayer1 = session.Player1Id != userId;

            session = await (new SessionListHandlerDb(_context)).AddSession(session);

            var player1 = await _userListHandler.GetUser(session.Player1Id);
            var player2 = await _userListHandler.GetUser(session.Player2Id);
            string responseDataJson = JsonConvert.SerializeObject(new SessionDto(session, player1, player2));
            await _notificationsService.SendNotificationAsync(responseDataJson, $"session{sessionId}");
            return Ok(responseDataJson);
        }

        private async Task<Result<Session>> getSession(int sessionId)
        {
            if (sessionId < 1)
                return Result.Fail("Wrong session index. It must be greater than 1");
            Session session = await(new SessionListHandlerDb(_context)).GetSession(sessionId);
            if (session == null)
                return Result.Fail("Game session not found");
            return session;

        }
    }
}
