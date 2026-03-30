using MedSync_API.Repositories;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de reportes financieros y operativos.
    /// Genera información consolidada para el administrador y médicos (RF19).
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IPaymentRepository _pagoRepo;

        public ReportService(IPaymentRepository pagoRepo)
        {
            _pagoRepo = pagoRepo;
        }

        /// <inheritdoc/>
        public async Task<object> ObtenerReporteIngresosAsync(int hospitalId, DateTime? desde = null, DateTime? hasta = null)
        {
            // Suma de todos los pagos completados en el rango de fechas indicado (RF19)
            var totalIngresos = await _pagoRepo.ObtenerTotalIngresosAsync(hospitalId, desde, hasta);

            return new
            {
                HospitalId     = hospitalId,
                TotalIngresos  = totalIngresos,
                Moneda         = "DOP",
                Desde          = desde,
                Hasta          = hasta ?? DateTime.UtcNow,
                GeneradoEn     = DateTime.UtcNow
            };
        }
    }
}
