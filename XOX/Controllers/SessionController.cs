using FluentResults;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using XOX.BLObjects;
using XOX.Enums;
using XOX.Services;

namespace XOX.Controllers
{
    [Route("session")]
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

        [HttpGet, Route("get")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            _cookies.AcquireClientId(HttpContext);
            var sessionResult = await getSession(sessionId);
            if (sessionResult.IsFailed)
                return BadRequest(sessionResult.Errors[0].Message);
            var session = sessionResult.Value;
            var responseData = new SessionDto(session);
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
            var clientId = GetClientId();
            var userId = AcquireUserId();
            var userResult = await BLObjects.User.GetOrCreate(userId);
            if (userResult.IsFailed)
                return BadRequest(userResult.Errors[0].Message);

            var user = userResult.Value;
            var session = new Session(user);
            await session.Save();
            _clientService.AddUserToGroup(clientId, $"session{session.Id}");
            var responseData = new SessionDto(session);
            return Ok(JsonConvert.SerializeObject(responseData));
        }

        [HttpPost, Route("connect")]
        public async Task<IActionResult> Connect(int sessionId)
        {
            var sessionResult = await getSession(sessionId);
            if (sessionResult.IsFailed)
                return BadRequest(sessionResult.Errors[0].Message);
            var session = sessionResult.Value;

            var clientId = GetClientId();
            var userId = AcquireUserId();
            var userResult = await BLObjects.User.GetOrCreate(userId);
            if (userResult.IsFailed)
                return BadRequest(userResult.Errors[0].Message);

            var user = userResult.Value;
            //If no empty slots
            if (!((session.Player1 == null || session.Player2 == null) || 
                (session.Player1.Id == Guid.Empty || session.Player2.Id == Guid.Empty) ||
                (session.Player1.Id == userId || session.Player2.Id == user.Id)))
                return NotFound("Cannot connect. There are no empty slots for players in the session");

            if ((session.Player1 == null || session.Player1.Id == Guid.Empty) && session.Player2.Id != user.Id)
            {
                if (session.Player2.Mark == user.Mark)
                {
                    return BadRequest("You have the same mark as the other player. Change your mark and try again");
                }
                session.Player1 = user;
            }
            else if ((session.Player2 == null || session.Player2.Id == Guid.Empty) && session.Player1.Id != user.Id)
            {
                if (session.Player1.Mark == user.Mark)
                {
                    return BadRequest("You have the same mark as the other player. Change your mark and try again");
                }
                session.Player2 = user;
            }

            sessionResult = await session.Save();

            if (sessionResult.IsFailed)
                return BadRequest(sessionResult.Errors[0].Message);

            session = sessionResult.Value;
            _clientService.AddUserToGroup(clientId, $"session{session.Id}");
            string responseDataJson = JsonConvert.SerializeObject(new SessionDto(session));
            await SendResponseForSession(session, userId, responseDataJson);

            return Ok(responseDataJson);
        }

        [HttpPost, Route("setMark")]
        public async Task<IActionResult> SetMark(int sessionId, int x, int y)
        {
            var sessionResult = await getSession(sessionId);
            if (sessionResult.IsFailed)
                return BadRequest(sessionResult.Errors[0].Message);
            var session = sessionResult.Value;
            if (session.State != SessionState.InProgress && session.State != SessionState.NotStarted)
                return BadRequest("Game session is finished or not found");

            var userId = AcquireUserId();
            if (session.Player1.Id == userId && (session.Player2 == null || session.Player2.Id == Guid.Empty) || 
                session.Player2.Id == userId && (session.Player1 == null || session.Player1.Id == Guid.Empty))
                return BadRequest("Can't start without 2nd player");
            //If no empty slots
            if (!((session.Player1 == null || session.Player2 == null) ||
                (session.Player1.Id == userId || session.Player2.Id == userId)))
                return Unauthorized("You not participate in game. Watch-only");

            if ((session.IsActivePlayer1 && session.Player1.Id != userId) ||
                (!session.IsActivePlayer1 && session.Player2.Id != userId))
                return BadRequest("The action is forbidden. It's not your turn");
            if (session.Field.Cells[x, y].Value != string.Empty)
                return BadRequest("The cell is alredy filled. Try another one");

            var userResult = await BLObjects.User.GetOrCreate(userId);
            if (userResult.IsFailed)
                return BadRequest(userResult.Errors[0].Message);

            var user = userResult.Value;
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

            await session.Save();

            string responseDataJson = JsonConvert.SerializeObject(new SessionDto(session));
            await SendResponseForSession(session, userId, responseDataJson);
            return Ok(responseDataJson);
        }

        [HttpPost, Route("retreat")]
        public async Task<IActionResult> FinishSession(int sessionId)
        {
            var userId = AcquireUserId();
            var sessionResult = await getSession(sessionId);
            if (sessionResult.IsFailed)
                return BadRequest(sessionResult.Errors[0].Message);
            var session = sessionResult.Value;

            if (session.Player1 == null || session.Player2 == null)
            {
                return Unauthorized("Game not started. You cannot retreat until both players connect");
            }
            if (userId != session.Player1.Id && userId != session.Player2.Id)
                return Unauthorized("You not participate in game. Watch-only");
            session.State = SessionState.Finished;
            session.IsActivePlayer1 = session.Player1.Id != userId;

            await session.Save();

            string responseDataJson = JsonConvert.SerializeObject(new SessionDto(session));
            await SendResponseForSession(session, userId, responseDataJson);
            return Ok(responseDataJson);
        }

        private static async Task<Result<Session>> getSession(int sessionId)
        {
            if (sessionId < 1)
                return Result.Fail("Wrong session index. It must be greater than 1");
            var session = await new Session().Get(sessionId);
            return session;

        }

        private async Task SendResponseForSession(Session session, Guid user, string responseData)
        {
            await _notificationsService.SendNotificationAsync(responseData, $"session{session.Id}");
            if (session.Player1 != null && session.Player1.Id != Guid.Empty)
                await _notificationsService.SendNotificationAsync(responseData, $"user{session.Player1.Id}");
            if (session.Player2 != null && session.Player2.Id != Guid.Empty)
                await _notificationsService.SendNotificationAsync(responseData, $"user{session.Player2.Id}");
            if (!session.HasUser(user))
                await _notificationsService.SendNotificationAsync(responseData, $"user{user}");
        }

        private Guid AcquireUserId()
        {
            Guid clientId;
            string COOKIE_NAME = ".ServerSentEvents.Guid";

            string cookieValue = HttpContext.Request.Cookies[COOKIE_NAME];
            if (string.IsNullOrWhiteSpace(cookieValue) || !Guid.TryParse(cookieValue, out clientId))
            {
                clientId = Guid.NewGuid();

                HttpContext.Response.Cookies.Append(COOKIE_NAME, clientId.ToString());
            }

            return clientId;
        }

        private Guid GetClientId()
        {
            Guid clientId;
            string clientIdHeader = "ClientId";

            string headerValue = HttpContext.Request.Headers[clientIdHeader];
            if (string.IsNullOrWhiteSpace(headerValue) || !Guid.TryParse(headerValue, out clientId))
            {
                clientId = Guid.Empty;
            }

            return clientId;
        }
    }
}
