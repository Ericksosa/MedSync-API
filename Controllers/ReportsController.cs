using MedSync_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ReportsController(ApplicationDbContext context) => _context = context;

    // GET: api/Reports/hospital/1/revenue (Rendimiento financiero)
    [HttpGet("hospital/{id}/revenue")]
    public async Task<IActionResult> GetRevenue(int id)
    {
        var totalRevenue = await _context.Payments
            .Where(p => p.Appointment!.HospitalId == id && p.Status == "Completed")
            .SumAsync(p => p.Amount);

        return Ok(new { HospitalId = id, TotalRevenue = totalRevenue, Currency = "DOP" });
    }
}