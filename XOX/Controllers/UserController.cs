using Microsoft.AspNetCore.Mvc;
using XOX.BLObjects;
using Newtonsoft.Json;
using System;
using Lib.AspNetCore.ServerSentEvents;
using System.Globalization;
using XOX.Database;
using System.Threading.Tasks;

namespace XOX.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private IServerSentEventsClientIdProvider _cookies;
        private UserListHandlerDb _userListHandler;
        private SessionContext _context;

        public UserController(IServerSentEventsClientIdProvider cookies, SessionContext context) { 
            _cookies = cookies;
            _context = context;
            _userListHandler = new UserListHandlerDb(_context);
        }

        [HttpGet, Route("get")]
        public async Task<IActionResult> GetUser()
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = await _userListHandler.GetUser(userId);
            if (user == null)
                return NotFound("Игрок не найден");
            return Ok(JsonConvert.SerializeObject(user));
        }

        [HttpGet, Route("getOrCreate")]
        public async Task<IActionResult> GetOrCreateUser()
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = await _userListHandler.GetUser(userId);
            if (user == null)
                user = await _userListHandler.AddUser(new User(userId));
            return Ok(JsonConvert.SerializeObject(user));
        }

        [HttpPost, Route("change")]
        public async Task<IActionResult> Change(string name, string mark)
        {
            if (new StringInfo(mark).LengthInTextElements > 1)
                return BadRequest("Изменение не выполнено. Метка должна быть 1 символом");
            if (name.Length > 50)
                return BadRequest("Изменение не выполнено. Имя должно быть не длиннее 50 символов");
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = await _userListHandler.GetUser(userId);
            if (user == null)
                user = new User(userId, name, mark);
            else if (user.HasActiveSessions)
            {
                //TODO: i don't store mark for different sessions, so i don't want to think about update for now
                return BadRequest("Нельзя сделать, пока есть активные сессии");
            }
            else
            {
                if(!string.IsNullOrEmpty(name))
                    user.Name = name;
                if (!string.IsNullOrEmpty(mark))
                    user.Mark = mark;
            }
            await _userListHandler.AddUser(user);
            return Ok(JsonConvert.SerializeObject(new User(userId, name, mark)));
        }
    }
}
