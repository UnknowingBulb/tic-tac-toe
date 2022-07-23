using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;
using XOX.BLObjects;
using XOX.Services;

namespace XOX.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private IClientService _clientService;
        private IServerSentEventsClientIdProvider _cookies;

        public UserController(IClientService clientService, IServerSentEventsClientIdProvider cookies) {
            _clientService = clientService;
            _cookies = cookies;
        }

        [HttpGet, Route("get")]
        public async Task<IActionResult> GetUser()
        {
            var userId = AcquireUserId();

            var userResult = await new User().Get(userId);
            if (userResult.IsFailed)
                return NotFound(userResult.Errors[0].Message);

            return Ok(JsonConvert.SerializeObject(userResult.Value));
        }

        [HttpGet, Route("getOrCreate")]
        public async Task<IActionResult> GetOrCreateUser()
        {
            var userId = AcquireUserId();
            var userResult = await BLObjects.User.GetOrCreate(userId);
            if (userResult.IsFailed)
                return BadRequest(userResult.Errors[0].Message);

            var user = userResult.Value;
            return Ok(JsonConvert.SerializeObject(user));
        }

        [HttpPost, Route("addClient")]
        public IActionResult AddClient()
        {
            var userId = AcquireUserId();
            var clientId = GetClientId();
            UserClientPool.AddClient(clientId, userId);
            _clientService.AddUserToGroup(clientId, $"user{userId}");
            return Ok();
        }

        [HttpPost, Route("change")]
        public async Task<IActionResult> Change(string name, string mark)
        {
            if (new StringInfo(mark).LengthInTextElements > 1)
                return BadRequest("Change wasn't applied. Mark should be one symbol");
            if (name.Length > 50)
                return BadRequest("Change wasn't applied. Name lenght should be 50 symbols or less");
            var userId = AcquireUserId();

            var user = new User();
            var userResult = await user.Get(userId);
            if (userResult.IsFailed)
                user = null;
            else
                user = userResult.Value;

            if (user == null)
                user = new User(userId, name, mark);
            else if (user.HasActiveSessions)
            {
                //TODO: i don't store mark for different sessions, so i don't want to think about update for now
                return BadRequest("Action is forbidden while you have active sessions");
            }
            else
            {
                if(!string.IsNullOrEmpty(name))
                    user.Name = name;
                if (!string.IsNullOrEmpty(mark))
                    user.Mark = mark;
            }
            await user.Save();
            return Ok(JsonConvert.SerializeObject(new User(userId, name, mark)));
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
