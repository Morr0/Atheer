using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers.Health
{
    [Route("/Health")]
    [ApiController]
    // TODO allow certain endpoints to only be accessed from within the same network
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult HealthyPublic()
        {
            return Ok();
        }
    }
}