using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class SpecialtiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public SpecialtiesController(ApplicationDbContext context) => _context = context;

    [HttpGet("hospital/{hospitalId}")]
    public async Task<ActionResult<IEnumerable<Specialty>>> GetSpecialties(int hospitalId) =>
        await _context.Specialties.Where(s => s.HospitalId == hospitalId).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Specialty>> PostSpecialty(Specialty specialty)
    {
        _context.Specialties.Add(specialty);
        await _context.SaveChangesAsync();
        return Ok(specialty);
    }
}