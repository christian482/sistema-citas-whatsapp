using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using citas.Data;
using citas.Models;

namespace citas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Doctors.Include(d => d.Appointments).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .ThenInclude(a => a.Client)
                .Include(d => d.Appointments)
                .ThenInclude(a => a.Service)
                .Include(d => d.Appointments)
                .ThenInclude(a => a.Business)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound(new { error = $"Doctor con ID {id} no encontrado" });

            return Ok(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            // Limpiar las propiedades de navegación que pueden venir del frontend para evitar conflictos de validación
            doctor.Appointments = null;
            
            if (string.IsNullOrWhiteSpace(doctor.Name))
                return BadRequest(new { error = "El nombre del doctor es requerido" });

            if (string.IsNullOrWhiteSpace(doctor.Specialty))
                return BadRequest(new { error = "La especialidad del doctor es requerida" });

            // Verificar si ya existe un doctor con el mismo nombre
            var existingDoctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Name.ToLower() == doctor.Name.ToLower());

            if (existingDoctor != null)
                return BadRequest(new { error = "Ya existe un doctor con ese nombre" });

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Doctor doctor)
        {
            // Permitir que el ID del doctor sea 0 o null desde el frontend
            // Solo validar si se proporciona un ID y no coincide
            if (doctor.Id != 0 && id != doctor.Id)
                return BadRequest(new { error = "El ID no coincide" });

            var existingDoctor = await _context.Doctors.FindAsync(id);
            if (existingDoctor == null)
                return NotFound(new { error = $"Doctor con ID {id} no encontrado" });

            if (string.IsNullOrWhiteSpace(doctor.Name))
                return BadRequest(new { error = "El nombre del doctor es requerido" });

            if (string.IsNullOrWhiteSpace(doctor.Specialty))
                return BadRequest(new { error = "La especialidad del doctor es requerida" });

            // Verificar duplicados (excluyendo el doctor actual)
            var duplicateDoctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id != id && d.Name.ToLower() == doctor.Name.ToLower());

            if (duplicateDoctor != null)
                return BadRequest(new { error = "Ya existe otro doctor con ese nombre" });

            // Actualizar propiedades (mantener el ID original)
            existingDoctor.Name = doctor.Name;
            existingDoctor.Specialty = doctor.Specialty;
            // Asegurar que el ID se mantenga
            existingDoctor.Id = id;

            await _context.SaveChangesAsync();
            return Ok(existingDoctor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound(new { error = $"Doctor con ID {id} no encontrado" });

            // Verificar si tiene citas activas
            var activeAppointments = doctor.Appointments.Where(a => !a.IsCancelled && a.Date > DateTime.Now).ToList();
            if (activeAppointments.Any())
            {
                return BadRequest(new { error = $"No se puede eliminar el doctor porque tiene {activeAppointments.Count} cita(s) activa(s) programada(s)" });
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Doctor '{doctor.Name}' eliminado correctamente" });
        }
    }
}
