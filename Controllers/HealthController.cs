using Microsoft.AspNetCore.Mvc;

namespace citas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "OK", 
                timestamp = DateTime.UtcNow,
                service = "Sistema de Citas",
                version = "1.0.0"
            });
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("pong");
    }
}
