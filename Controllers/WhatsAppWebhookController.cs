using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;
using citas.Services;
using citas.Data;
using citas.Models;
using Microsoft.EntityFrameworkCore;

namespace citas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WhatsAppWebhookController : ControllerBase
    {
        private readonly WhatsAppService _whatsAppService;
        private readonly AppDbContext _db;
        public WhatsAppWebhookController(WhatsAppService whatsAppService, AppDbContext db)
        {
            _whatsAppService = whatsAppService;
            _db = db;
        }

        [HttpGet("privacidad")]
        public IActionResult Privacidad() => Content("Este sistema almacena datos mínimos para agendar citas, nunca compartidos con terceros.", "text/plain");

        [HttpGet]
        public IActionResult VerifyWebhook([FromQuery(Name = "hub.mode")] string mode, 
                                         [FromQuery(Name = "hub.challenge")] string challenge, 
                                         [FromQuery(Name = "hub.verify_token")] string verifyToken)
        {
            var expectedToken = "mi_token_secreto"; // Cambia por tu token de verificación
            if (mode == "subscribe" && verifyToken == expectedToken)
                return Content(challenge, "text/plain");
            return Forbid();
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] JsonElement body)
        {
            // Extraer datos básicos del mensaje entrante
            var entry = body.GetProperty("entry")[0];
            var changes = entry.GetProperty("changes")[0];
            var value = changes.GetProperty("value");
            var messages = value.TryGetProperty("messages", out var msgs) ? msgs : default;
            if (messages.ValueKind != JsonValueKind.Array || messages.GetArrayLength() == 0)
                return Ok();

            var msg = messages[0];
            var from = msg.GetProperty("from").GetString();
            var text = msg.GetProperty("text").GetProperty("body").GetString()?.Trim().ToLower();

            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(text))
                return Ok();

            string response = "No entendí tu mensaje. Escribe 'Agendar', 'Cancelar' o 'Mis citas'.";

            if (text.StartsWith("agendar"))
            {
                // Flujo de agendar cita
                // Ejemplo: "Agendar 2025-10-01 10:00 Dr. Juan"
                var parts = text.Split(' ');
                if (parts.Length < 4)
                {
                    response = "Formato inválido. Ejemplo: Agendar 2025-10-01 10:00 Dr. Juan";
                }
                else
                {
                    var dateStr = parts[1] + " " + parts[2];
                    if (!DateTime.TryParse(dateStr, out var date))
                    {
                        response = "Fecha y hora inválidas. Usa formato: Agendar YYYY-MM-DD HH:mm Dr. Nombre";
                    }
                    else
                    {
                        var doctorName = string.Join(' ', parts.Skip(3));
                        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Name.ToLower() == doctorName.ToLower());
                        if (doctor == null)
                        {
                            response = $"No se encontró el médico '{doctorName}'.";
                        }
                        else
                        {
                            // Buscar cliente por teléfono
                            var client = await _db.Clients.FirstOrDefaultAsync(c => c.Phone == from);
                            if (client == null)
                            {
                                client = new Client { Name = from ?? "", Phone = from ?? "", Email = "" };
                                _db.Clients.Add(client);
                                await _db.SaveChangesAsync();
                            }
                            // Verificar disponibilidad
                            var exists = await _db.Appointments.AnyAsync(a => a.DoctorId == doctor.Id && a.Date == date && !a.IsCancelled);
                            if (exists)
                            {
                                response = $"El médico ya tiene una cita en ese horario. Elige otro.";
                            }
                            else
                            {
                                // Asignar consultorio y servicio por defecto
                                var business = await _db.Businesses.FirstOrDefaultAsync();
                                var service = await _db.Services.FirstOrDefaultAsync();
                                var appointment = new Appointment
                                {
                                    Date = date,
                                    DoctorId = doctor.Id,
                                    ClientId = client.Id,
                                    BusinessId = business?.Id ?? 1,
                                    ServiceId = service?.Id ?? 1,
                                    IsCancelled = false
                                };
                                _db.Appointments.Add(appointment);
                                await _db.SaveChangesAsync();
                                response = $"Cita agendada con el Dr. {doctor.Name} el {date}.";
                            }
                        }
                    }
                }
            }
            else if (text.StartsWith("cancelar"))
            {
                // Flujo de cancelar cita
                // Ejemplo: "Cancelar 2025-10-01 10:00"
                var parts = text.Split(' ');
                if (parts.Length < 3)
                {
                    response = "Formato inválido. Ejemplo: Cancelar 2025-10-01 10:00";
                }
                else
                {
                    var dateStr = parts[1] + " " + parts[2];
                    if (!DateTime.TryParse(dateStr, out var date))
                    {
                        response = "Fecha y hora inválidas. Usa formato: Cancelar YYYY-MM-DD HH:mm";
                    }
                    else
                    {
                        var client = await _db.Clients.FirstOrDefaultAsync(c => c.Phone == from);
                        if (client == null)
                        {
                            response = "No tienes citas registradas.";
                        }
                        else
                        {
                            var appt = await _db.Appointments.FirstOrDefaultAsync(a => a.ClientId == client.Id && a.Date == date && !a.IsCancelled);
                            if (appt == null)
                            {
                                response = "No se encontró una cita tuya en ese horario.";
                            }
                            else
                            {
                                appt.IsCancelled = true;
                                await _db.SaveChangesAsync();
                                response = "Cita cancelada exitosamente.";
                            }
                        }
                    }
                }
            }
            else if (text.StartsWith("mis citas"))
            {
                // Flujo de consultar citas
                var client = await _db.Clients.FirstOrDefaultAsync(c => c.Phone == from);
                if (client == null)
                {
                    response = "No tienes citas registradas.";
                }
                else
                {
                    var citas = await _db.Appointments
                        .Include(a => a.Doctor)
                        .Where(a => a.ClientId == client.Id && !a.IsCancelled && a.Date > DateTime.Now)
                        .OrderBy(a => a.Date)
                        .ToListAsync();
                    if (!citas.Any())
                    {
                        response = "No tienes citas próximas.";
                    }
                    else
                    {
                        response = "Tus próximas citas:\n" + string.Join("\n", citas.Select(a => $"{a.Date:yyyy-MM-dd HH:mm} con Dr. {a.Doctor.Name}"));
                    }
                }
            }
            else if (text.StartsWith("horarios"))
            {
                // Flujo de consultar horarios disponibles de un médico
                // Ejemplo: "Horarios Dr. Juan 2025-10-01"
                var parts = text.Split(' ');
                if (parts.Length < 3)
                {
                    response = "Formato inválido. Ejemplo: Horarios Dr. Nombre YYYY-MM-DD";
                }
                else
                {
                    var doctorName = string.Join(' ', parts.Skip(1).Take(parts.Length - 2));
                    var dateStr = parts.Last();
                    if (!DateTime.TryParse(dateStr, out var date))
                    {
                        response = "Fecha inválida. Usa formato: Horarios Dr. Nombre YYYY-MM-DD";
                    }
                    else
                    {
                        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Name.ToLower() == doctorName.ToLower());
                        if (doctor == null)
                        {
                            response = $"No se encontró el médico '{doctorName}'.";
                        }
                        else
                        {
                            // Ejemplo: horarios de 9 a 18 cada hora
                            var horarios = Enumerable.Range(9, 10).Select(h => date.Date.AddHours(h)).ToList();
                            var ocupados = await _db.Appointments.Where(a => a.DoctorId == doctor.Id && a.Date.Date == date.Date && !a.IsCancelled).Select(a => a.Date).ToListAsync();
                            var libres = horarios.Except(ocupados).ToList();
                            if (!libres.Any())
                                response = "No hay horarios disponibles para ese día.";
                            else
                                response = "Horarios disponibles:\n" + string.Join("\n", libres.Select(h => h.ToString("HH:mm")));
                        }
                    }
                }
            }

            // Responder al usuario por WhatsApp
            if (!string.IsNullOrEmpty(from))
                await _whatsAppService.SendMessageAsync(from, response);
            return Ok();
        }
    }
}
