using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public PaymentsController(ApplicationDbContext context) => _context = context;

    // POST: api/Payments (Procesar pago de cita)
    [HttpPost]
    public async Task<ActionResult<Payment>> PostPayment(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return Ok(payment);
    }

    // GET: api/Payments/hospital/1 (Historial para auditoría)
    [HttpGet("hospital/{hospitalId}")]
    public async Task<ActionResult<IEnumerable<Payment>>> GetHospitalPayments(int hospitalId)
    {
        return await _context.Payments
            .Where(p => p.Appointment!.HospitalId == hospitalId)
            .ToListAsync();
    }

    // DELETE: api/Payments/5 (Eliminar registro erróneo - Requisito de verbo DELETE)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return NotFound();
        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}