using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using citas.Data;
using citas.Models;

namespace citas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BusinessController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Businesses.Include(b => b.Services).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var business = await _context.Businesses
                .Include(b => b.Services)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (business == null)
                return NotFound(new { error = $"Negocio con ID {id} no encontrado" });

            return Ok(business);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Business business)
        {
            // Limpiar las propiedades de navegación que pueden venir del frontend para evitar conflictos de validación
            business.Services = null;
            
            if (string.IsNullOrWhiteSpace(business.Name))
                return BadRequest(new { error = "El nombre del negocio es requerido" });

            if (string.IsNullOrWhiteSpace(business.WorkingHours))
                return BadRequest(new { error = "Las horas de trabajo son requeridas" });

            if (string.IsNullOrWhiteSpace(business.WorkingDays))
                return BadRequest(new { error = "Los días de trabajo son requeridos" });

            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = business.Id }, business);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Business business)
        {
            // Limpiar las propiedades de navegación que pueden venir del frontend para evitar conflictos de validación
            business.Services = null;
            
            // Permitir que el ID del negocio sea 0 o null desde el frontend
            // Solo validar si se proporciona un ID y no coincide
            if (business.Id != 0 && id != business.Id)
                return BadRequest(new { error = "El ID no coincide" });

            var existingBusiness = await _context.Businesses.FindAsync(id);
            if (existingBusiness == null)
                return NotFound(new { error = $"Negocio con ID {id} no encontrado" });

            if (string.IsNullOrWhiteSpace(business.Name))
                return BadRequest(new { error = "El nombre del negocio es requerido" });

            if (string.IsNullOrWhiteSpace(business.WorkingHours))
                return BadRequest(new { error = "Las horas de trabajo son requeridas" });

            if (string.IsNullOrWhiteSpace(business.WorkingDays))
                return BadRequest(new { error = "Los días de trabajo son requeridos" });

            // Actualizar propiedades (mantener el ID original)
            existingBusiness.Name = business.Name;
            existingBusiness.WorkingHours = business.WorkingHours;
            existingBusiness.WorkingDays = business.WorkingDays;
            // Asegurar que el ID se mantenga
            existingBusiness.Id = id;

            await _context.SaveChangesAsync();
            return Ok(existingBusiness);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var business = await _context.Businesses
                .Include(b => b.Services)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (business == null)
                return NotFound(new { error = $"Negocio con ID {id} no encontrado" });

            if (business.Services?.Any() == true)
            {
                return BadRequest(new { error = $"No se puede eliminar el negocio porque tiene {business.Services.Count} servicio(s) asociado(s)" });
            }

            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Negocio '{business.Name}' eliminado correctamente" });
        }
    }
}
