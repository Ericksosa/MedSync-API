using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Payment.
    /// Extiende las operaciones CRUD con consultas financieras del módulo (RF16, RF19).
    /// </summary>
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        /// <summary>Obtiene todos los pagos registrados en un hospital específico.</summary>
        Task<IEnumerable<Payment>> ObtenerPorHospitalAsync(int hospitalId);

        /// <summary>Calcula el total de ingresos completados de un hospital en un rango de fechas.</summary>
        Task<decimal> ObtenerTotalIngresosAsync(int hospitalId, DateTime? desde = null, DateTime? hasta = null);

        /// <summary>Obtiene el pago asociado a una cita específica.</summary>
        Task<Payment?> ObtenerPorCitaAsync(int citaId);

        /// <summary>Calcula el total de ingresos completados generados por un médico.</summary>
        Task<decimal> ObtenerTotalGeneradoPorDoctorAsync(int doctorId);
    }
}
