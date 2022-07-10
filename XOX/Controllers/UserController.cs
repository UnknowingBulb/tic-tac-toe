using Microsoft.AspNetCore.Mvc;
using XOX.BLObjects;
using Newtonsoft.Json;
using System;
using Lib.AspNetCore.ServerSentEvents;
using System.Globalization;

namespace XOX.Controllers
{
    public class UserController : Controller
    {
        private IServerSentEventsClientIdProvider _cookies;

        public UserController(IServerSentEventsClientIdProvider cookies) { 
            _cookies = cookies;
        }

        [HttpGet, Route("/get")]
        public IActionResult GetUser()
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.GetUser(userId);
            if (user == null)
                return NotFound("Игрок не найден");
            return Ok(JsonConvert.SerializeObject(user));
        }

        [HttpGet, Route("/getOrCreate")]
        public IActionResult GetOrCreateUser()
        {
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.GetUser(userId);
            if (user == null)
                user = UserListHandler.AddUser(new User(userId));
            return Ok(JsonConvert.SerializeObject(user));
        }

        [HttpPost, Route("/change")]
        public IActionResult Change(string name, string mark)
        {
            if (new StringInfo(mark).LengthInTextElements > 1)
                return BadRequest("Изменение не выполнено. Метка должна быть 1 символом");
            if (name.Length > 50)
                return BadRequest("Изменение не выполнено. Имя должно быть не длиннее 50 символов");
            Guid userId = _cookies.AcquireClientId(HttpContext);
            User user = UserListHandler.GetUser(userId);
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
            UserListHandler.AddUser(user);
            return Ok(JsonConvert.SerializeObject(new User(userId, name, mark)));
        }

    }
}
