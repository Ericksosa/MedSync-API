using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class DoctorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public DoctorsController(ApplicationDbContext context) => _context = context;

    // GET: api/Doctors/hospital/1 (Listar empleados por hospital)
    [HttpGet("hospital/{hospitalId}")]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors(int hospitalId)
    {
        return await _context.Doctors
            .Where(d => d.HospitalId == hospitalId)
            .Include(d => d.Specialty)
            .ToListAsync();
    }

    // POST: api/Doctors (Contratar/Registrar médico)
    [HttpPost]
    public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return Ok(doctor);
    }

    // PATCH: api/Doctors/5/status (Cambiar estado operativo)
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> PatchDoctorStatus(int id, [FromBody] bool isActive)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return NotFound();
        doctor.IsActive = isActive;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}