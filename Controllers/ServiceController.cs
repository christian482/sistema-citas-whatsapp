using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using citas.Data;
using citas.Models;

namespace citas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Services.Include(s => s.Business).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _context.Services
                .Include(s => s.Business)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound(new { error = $"Servicio con ID {id} no encontrado" });

            return Ok(service);
        }

        [HttpGet("business/{businessId}")]
        public async Task<IActionResult> GetByBusiness(int businessId)
        {
            var services = await _context.Services
                .Include(s => s.Business)
                .Where(s => s.BusinessId == businessId)
                .ToListAsync();

            return Ok(services);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Service service)
        {
            // Limpiar la propiedad Business que puede venir del frontend para evitar conflictos de validación
            service.Business = null;
            
            if (string.IsNullOrWhiteSpace(service.Name))
                return BadRequest(new { error = "El nombre del servicio es requerido" });

            // Verificar que el negocio existe
            var business = await _context.Businesses.FindAsync(service.BusinessId);
            if (business == null)
                return BadRequest(new { error = "El negocio especificado no existe" });

            // Verificar si ya existe un servicio con el mismo nombre en el mismo negocio
            var existingService = await _context.Services
                .FirstOrDefaultAsync(s => s.BusinessId == service.BusinessId && 
                                        s.Name.ToLower() == service.Name.ToLower());

            if (existingService != null)
                return BadRequest(new { error = "Ya existe un servicio con ese nombre en este negocio" });

            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Service service)
        {
            // Limpiar la propiedad Business que puede venir del frontend para evitar conflictos de validación
            service.Business = null;
            
            // Permitir que el ID del servicio sea 0 o null desde el frontend
            // Solo validar si se proporciona un ID y no coincide
            if (service.Id != 0 && id != service.Id)
                return BadRequest(new { error = "El ID no coincide" });

            var existingService = await _context.Services.FindAsync(id);
            if (existingService == null)
                return NotFound(new { error = $"Servicio con ID {id} no encontrado" });

            if (string.IsNullOrWhiteSpace(service.Name))
                return BadRequest(new { error = "El nombre del servicio es requerido" });

            // Verificar que el negocio existe
            var business = await _context.Businesses.FindAsync(service.BusinessId);
            if (business == null)
                return BadRequest(new { error = "El negocio especificado no existe" });

            // Verificar duplicados (excluyendo el servicio actual)
            var duplicateService = await _context.Services
                .FirstOrDefaultAsync(s => s.Id != id && 
                                        s.BusinessId == service.BusinessId && 
                                        s.Name.ToLower() == service.Name.ToLower());

            if (duplicateService != null)
                return BadRequest(new { error = "Ya existe otro servicio con ese nombre en este negocio" });

            // Actualizar propiedades (mantener el ID original)
            existingService.Name = service.Name;
            existingService.BusinessId = service.BusinessId;
            // Asegurar que el ID se mantenga
            existingService.Id = id;

            await _context.SaveChangesAsync();
            return Ok(existingService);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound(new { error = $"Servicio con ID {id} no encontrado" });

            // Verificar si tiene citas asociadas
            var appointmentsCount = await _context.Appointments
                .CountAsync(a => a.ServiceId == id && !a.IsCancelled);

            if (appointmentsCount > 0)
            {
                return BadRequest(new { error = $"No se puede eliminar el servicio porque tiene {appointmentsCount} cita(s) activa(s) asociada(s)" });
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Servicio '{service.Name}' eliminado correctamente" });
        }
    }
}
