using Microsoft.AspNetCore.Mvc;

namespace TypingMaster.Server.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new { message = "Welcome to TypingMaster API" });
        }
    }
}