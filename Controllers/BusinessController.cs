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

        [HttpPost]
        public async Task<IActionResult> Create(Business business)
        {
            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = business.Id }, business);
        }
    }
}
