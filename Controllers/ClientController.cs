using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using citas.Data;
using citas.Models;

namespace citas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClientController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Clients.Include(c => c.Appointments).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Appointments)
                .ThenInclude(a => a.Service)
                .Include(c => c.Appointments)
                .ThenInclude(a => a.Doctor)
                .Include(c => c.Appointments)
                .ThenInclude(a => a.Business)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound(new { error = $"Cliente con ID {id} no encontrado" });

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            // Limpiar las propiedades de navegación que pueden venir del frontend para evitar conflictos de validación
            client.Appointments = null;
            
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(client.Name))
                return BadRequest(new { error = "El nombre es requerido" });

            if (string.IsNullOrWhiteSpace(client.Phone))
                return BadRequest(new { error = "El teléfono es requerido" });

            if (string.IsNullOrWhiteSpace(client.Email))
                return BadRequest(new { error = "El email es requerido" });

            // Verificar si ya existe un cliente con el mismo email o teléfono
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == client.Email || c.Phone == client.Phone);

            if (existingClient != null)
                return BadRequest(new { error = "Ya existe un cliente con ese email o teléfono" });

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Client client)
        {
            // Limpiar las propiedades de navegación que pueden venir del frontend para evitar conflictos de validación
            client.Appointments = null;
            
            // Permitir que el ID del cliente sea 0 o null desde el frontend
            // Solo validar si se proporciona un ID y no coincide
            if (client.Id != 0 && id != client.Id)
                return BadRequest(new { error = "El ID no coincide" });

            var existingClient = await _context.Clients.FindAsync(id);
            if (existingClient == null)
                return NotFound(new { error = $"Cliente con ID {id} no encontrado" });

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(client.Name))
                return BadRequest(new { error = "El nombre es requerido" });

            if (string.IsNullOrWhiteSpace(client.Phone))
                return BadRequest(new { error = "El teléfono es requerido" });

            if (string.IsNullOrWhiteSpace(client.Email))
                return BadRequest(new { error = "El email es requerido" });

            // Verificar duplicados (excluyendo el cliente actual)
            var duplicateClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id != id && (c.Email == client.Email || c.Phone == client.Phone));

            if (duplicateClient != null)
                return BadRequest(new { error = "Ya existe otro cliente con ese email o teléfono" });

            // Actualizar propiedades (mantener el ID original)
            existingClient.Name = client.Name;
            existingClient.Phone = client.Phone;
            existingClient.Email = client.Email;
            // Asegurar que el ID se mantenga
            existingClient.Id = id;

            await _context.SaveChangesAsync();
            return Ok(existingClient);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Appointments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound(new { error = $"Cliente con ID {id} no encontrado" });

            // Verificar si tiene citas activas
            var activeAppointments = client.Appointments?.Where(a => !a.IsCancelled && a.Date > DateTime.Now).ToList() ?? new List<Appointment>();
            if (activeAppointments.Any())
            {
                return BadRequest(new { error = $"No se puede eliminar el cliente porque tiene {activeAppointments.Count} cita(s) activa(s) programada(s)" });
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Cliente '{client.Name}' eliminado correctamente" });
        }
    }
}
