using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de pagos.
    /// Gestiona el acceso a datos de los cobros por consultas médicas (RF16, RF19).
    /// </summary>
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<IEnumerable<Payment>> ObtenerPorHospitalAsync(int hospitalId)
        {
            // Navega por la relación Payment → Appointment para filtrar por hospital
            return await _dbSet
                .Where(p => p.Appointment!.HospitalId == hospitalId)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Patient)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Doctor)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Hospital)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<decimal> ObtenerTotalIngresosAsync(int hospitalId, DateTime? desde = null, DateTime? hasta = null)
        {
            // Calcula el total de ingresos completados para el reporte financiero (RF19)
            var consulta = _dbSet
                .Where(p => p.Appointment!.HospitalId == hospitalId && p.Status == "Completed");

            if (desde.HasValue)
                consulta = consulta.Where(p => p.PaymentDate >= desde.Value);

            if (hasta.HasValue)
                consulta = consulta.Where(p => p.PaymentDate <= hasta.Value);

            return await consulta.SumAsync(p => p.Amount);
        }

        /// <inheritdoc/>
        public async Task<Payment?> ObtenerPorCitaAsync(int citaId)
        {
            // Obtiene el pago asociado a una cita específica (relación 1 a 1)
            return await _dbSet.FirstOrDefaultAsync(p => p.AppointmentId == citaId);
        }

        /// <inheritdoc/>
        public async Task<decimal> ObtenerTotalGeneradoPorDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Where(p => p.Appointment!.DoctorId == doctorId && p.Status == "Completed")
                .SumAsync(p => p.Amount);
        }
    }
}
