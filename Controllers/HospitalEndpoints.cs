using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedSync_API.Data;
using MedSync_API.Models;

namespace MedSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HospitalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Hospitals (Listar todos - Requisito funcional)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hospital>>> GetHospitals()
        {
            return await _context.Hospitals.ToListAsync();
        }

        // POST: api/Hospitals (Crear nuevo - Requisito funcional)
        [HttpPost]
        public async Task<ActionResult<Hospital>> PostHospital(Hospital hospital)
        {
            _context.Hospitals.Add(hospital);
            await _context.Set<Hospital>().AddAsync(hospital); // Uso de Set para persistencia
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHospitals), new { id = hospital.Id }, hospital);
        }

        // PUT: api/Hospitals/5 (Actualizar todo el registro)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHospital(int id, Hospital hospital)
        {
            if (id != hospital.Id) return BadRequest();
            _context.Entry(hospital).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Hospitals.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }
    }
}