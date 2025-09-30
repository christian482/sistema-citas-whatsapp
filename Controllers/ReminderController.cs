using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using citas.Data;
using citas.Models;

namespace citas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReminderController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint para enviar recordatorios manualmente (ejemplo)
        [HttpPost("send")] 
        public async Task<IActionResult> SendReminders()
        {
            var now = DateTime.UtcNow;
            var appointments = await _context.Appointments
                .Include(a => a.Client)
                .Where(a => !a.IsCancelled &&
                    (a.Date > now && (a.Date - now).TotalHours <= 24))
                .ToListAsync();
            // Aquí se llamaría al servicio de WhatsApp para enviar recordatorios
            return Ok(new { count = appointments.Count });
        }
    }
}
