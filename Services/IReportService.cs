namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de reportes.
    /// Genera reportes financieros y operativos del hospital (RF19).
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Genera el reporte de ingresos de un hospital en un rango de fechas.
        /// Disponible para el administrador y médicos del hospital (RF19).
        /// </summary>
        Task<object> ObtenerReporteIngresosAsync(int hospitalId, DateTime? desde = null, DateTime? hasta = null);
    }
}
