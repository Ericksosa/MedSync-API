using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador de reportes financieros y operativos (RF19).
    /// Genera estadísticas de ingresos por hospital en rangos de fecha configurables.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Doctor")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _servicio;

        public ReportsController(IReportService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Genera el reporte de ingresos de un hospital.
        /// Permite filtrar por rango de fechas mediante query strings (RF19).
        /// GET: api/reports/hospital/1/ingresos?desde=2026-01-01&hasta=2026-03-31
        /// </summary>
        [HttpGet("hospital/{hospitalId:int}/ingresos")]
        public async Task<IActionResult> ObtenerReporteIngresos(
            int hospitalId,
            [FromQuery] DateTime? desde = null,
            [FromQuery] DateTime? hasta = null)
        {
            var reporte = await _servicio.ObtenerReporteIngresosAsync(hospitalId, desde, hasta);
            return Ok(reporte);
        }
    }
}
