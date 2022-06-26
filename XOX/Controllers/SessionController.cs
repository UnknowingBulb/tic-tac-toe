using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XOX.Objects;
using Newtonsoft.Json;

namespace XOX.Controllers
{
    [ApiController]
    [Route("session")]
    public class SessionController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public SessionController(ILogger<WeatherForecastController> logger)
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

        [HttpPost, Route("/finish")]
        public IActionResult FinishSession(int sessionId)
        {
            SessionListHandler.Remove(sessionId);
            return Ok();
        }
    }
}
