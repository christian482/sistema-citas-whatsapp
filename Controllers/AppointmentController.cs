using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using citas.Data;
using citas.Models;

namespace citas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Appointments
            .Include(a => a.Service)
            .Include(a => a.Client)
            .Include(a => a.Doctor)
            .Include(a => a.Business)
            .OrderBy(a => a.Date)
            .ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Client)
                .Include(a => a.Doctor)
                .Include(a => a.Business)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound(new { error = $"Cita con ID {id} no encontrada" });

            return Ok(appointment);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClient(int clientId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Client)
                .Include(a => a.Doctor)
                .Include(a => a.Business)
                .Where(a => a.ClientId == clientId)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctor(int doctorId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Client)
                .Include(a => a.Doctor)
                .Include(a => a.Business)
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpGet("business/{businessId}")]
        public async Task<IActionResult> GetByBusiness(int businessId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Client)
                .Include(a => a.Doctor)
                .Include(a => a.Business)
                .Where(a => a.BusinessId == businessId)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetByDate(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Client)
                .Include(a => a.Doctor)
                .Include(a => a.Business)
                .Where(a => a.Date >= startDate && a.Date < endDate)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // Limpiar las propiedades de navegación que pueden venir del frontend para evitar conflictos de validación
            appointment.Service = null;
            appointment.Client = null;
            appointment.Doctor = null;
            appointment.Business = null;
            
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(appointment.Title))
                return BadRequest(new { error = "El título de la cita es requerido" });
                
            if (appointment.Date <= DateTime.Now)
                return BadRequest(new { error = "La fecha de la cita debe ser futura" });

            // Verificar que el cliente existe
            var client = await _context.Clients.FindAsync(appointment.ClientId);
            if (client == null)
                return BadRequest(new { error = "El cliente especificado no existe" });

            // Verificar que el servicio existe
            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null)
                return BadRequest(new { error = "El servicio especificado no existe" });

            // Verificar que el doctor existe
            var doctor = await _context.Doctors.FindAsync(appointment.DoctorId);
            if (doctor == null)
                return BadRequest(new { error = "El doctor especificado no existe" });

            // Verificar que el negocio existe
            var business = await _context.Businesses.FindAsync(appointment.BusinessId);
            if (business == null)
                return BadRequest(new { error = "El negocio especificado no existe" });

            // Verificar conflictos de horario para el doctor
            var conflictingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.DoctorId == appointment.DoctorId &&
                                        a.Date == appointment.Date &&
                                        !a.IsCancelled);

            if (conflictingAppointment != null)
                return BadRequest(new { error = "El doctor ya tiene una cita programada en esa fecha y hora" });

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Appointment appointment)
        {
            // Limpiar las propiedades de navegación que pueden venir del frontend para evitar conflictos de validación
            appointment.Service = null;
            appointment.Client = null;
            appointment.Doctor = null;
            appointment.Business = null;
            
            // Permitir que el ID de la cita sea 0 o null desde el frontend
            // Solo validar si se proporciona un ID y no coincide
            if (appointment.Id != 0 && id != appointment.Id)
                return BadRequest(new { error = "El ID no coincide" });

            var existingAppointment = await _context.Appointments.FindAsync(id);
            if (existingAppointment == null)
                return NotFound(new { error = $"Cita con ID {id} no encontrada" });

            if (existingAppointment.IsCancelled)
                return BadRequest(new { error = "No se puede modificar una cita cancelada" });

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(appointment.Title))
                return BadRequest(new { error = "El título de la cita es requerido" });
                
            if (appointment.Date <= DateTime.Now)
                return BadRequest(new { error = "La fecha de la cita debe ser futura" });

            // Verificar que el cliente existe
            var client = await _context.Clients.FindAsync(appointment.ClientId);
            if (client == null)
                return BadRequest(new { error = "El cliente especificado no existe" });

            // Verificar que el servicio existe
            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null)
                return BadRequest(new { error = "El servicio especificado no existe" });

            // Verificar que el doctor existe
            var doctor = await _context.Doctors.FindAsync(appointment.DoctorId);
            if (doctor == null)
                return BadRequest(new { error = "El doctor especificado no existe" });

            // Verificar que el negocio existe
            var business = await _context.Businesses.FindAsync(appointment.BusinessId);
            if (business == null)
                return BadRequest(new { error = "El negocio especificado no existe" });

            // Verificar conflictos de horario para el doctor (excluyendo la cita actual)
            var conflictingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id != id &&
                                        a.DoctorId == appointment.DoctorId &&
                                        a.Date == appointment.Date &&
                                        !a.IsCancelled);

            if (conflictingAppointment != null)
                return BadRequest(new { error = "El doctor ya tiene otra cita programada en esa fecha y hora" });

            // Actualizar propiedades
            existingAppointment.Title = appointment.Title;
            existingAppointment.Date = appointment.Date;
            existingAppointment.ClientId = appointment.ClientId;
            existingAppointment.ServiceId = appointment.ServiceId;
            existingAppointment.DoctorId = appointment.DoctorId;
            existingAppointment.BusinessId = appointment.BusinessId;

            await _context.SaveChangesAsync();
            return Ok(existingAppointment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) 
                return NotFound(new { error = $"Cita con ID {id} no encontrada" });

            if (appointment.IsCancelled)
                return BadRequest(new { error = "La cita ya está cancelada" });

            appointment.IsCancelled = true;
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                message = $"Cita de {appointment.Client?.Name} para {appointment.Service?.Name} cancelada correctamente",
                appointment = appointment
            });
        }

        [HttpPost("{id}/reactivate")]
        public async Task<IActionResult> Reactivate(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound(new { error = $"Cita con ID {id} no encontrada" });

            if (!appointment.IsCancelled)
                return BadRequest(new { error = "La cita ya está activa" });

            if (appointment.Date <= DateTime.Now)
                return BadRequest(new { error = "No se puede reactivar una cita pasada" });

            // Verificar conflictos de horario para el doctor
            var conflictingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id != id &&
                                        a.DoctorId == appointment.DoctorId &&
                                        a.Date == appointment.Date &&
                                        !a.IsCancelled);

            if (conflictingAppointment != null)
                return BadRequest(new { error = "El doctor ya tiene otra cita programada en esa fecha y hora" });

            appointment.IsCancelled = false;
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = $"Cita de {appointment.Client?.Name} reactivada correctamente",
                appointment = appointment
            });
        }
    }
}
