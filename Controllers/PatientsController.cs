using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedSync_API.Data;
using MedSync_API.Models;

namespace MedSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PatientsController(ApplicationDbContext context) => _context = context;

        // GET: api/Patients/40200000000 (Buscar por Cédula)
        [HttpGet("{documentId}")]
        public async Task<ActionResult<Patient>> GetPatient(string documentId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.DocumentId == documentId);
            if (patient == null) return NotFound();
            return patient;
        }

        // POST: api/Patients (Registrar cliente)
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPatient), new { documentId = patient.DocumentId }, patient);
        }

        // PUT: api/Patients/5 (Actualizar ficha técnica)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            if (id != patient.Id) return BadRequest();
            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}