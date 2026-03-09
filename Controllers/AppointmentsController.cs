using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentsController(ApplicationDbContext context) => _context = context;

    // GET: api/Appointments/hospital/1 (Análisis de demanda)
    [HttpGet("hospital/{hospitalId}")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByHospital(int hospitalId)
    {
        return await _context.Appointments
            .Where(a => a.HospitalId == hospitalId)
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ToListAsync();
    }

    // POST: api/Appointments (Reservar cita digitalmente)
    [HttpPost]
    public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return Ok(appointment);
    }

    // PATCH: api/Appointments/5/status (Sincronizar según el proceso)
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> PatchStatus(int id, [FromBody] string newStatus)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        appointment.Status = newStatus; // Ej: "Completada" o "Cancelada"
        await _context.SaveChangesAsync();
        return NoContent();
    }
}