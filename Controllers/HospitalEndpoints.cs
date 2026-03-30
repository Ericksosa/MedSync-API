using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador de hospitales y centros médicos (RF20).
    /// Solo los administradores pueden crear y actualizar hospitales.
    /// Los endpoints de consulta son accesibles para usuarios autenticados.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Todas las rutas requieren JWT válido (RNF04)
    public class HospitalsController : ControllerBase
    {
        private readonly IHospitalService _servicio;

        public HospitalsController(IHospitalService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Obtiene todos los hospitales registrados en el sistema.
        /// GET: api/hospitals
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HospitalViewModel>>> ObtenerTodos()
        {
            var hospitales = await _servicio.ObtenerTodosAsync();
            return Ok(hospitales);
        }

        /// <summary>
        /// Obtiene solo los hospitales activos (para uso en selectores de reserva).
        /// GET: api/hospitals/activos
        /// </summary>
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<HospitalViewModel>>> ObtenerActivos()
        {
            var hospitales = await _servicio.ObtenerActivosAsync();
            return Ok(hospitales);
        }

        /// <summary>
        /// Obtiene un hospital por su identificador.
        /// GET: api/hospitals/5
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<HospitalViewModel>> ObtenerPorId(int id)
        {
            var hospital = await _servicio.ObtenerPorIdAsync(id);
            if (hospital is null) return NotFound($"Hospital con id {id} no encontrado.");
            return Ok(hospital);
        }

        /// <summary>
        /// Registra un nuevo hospital en el sistema (RF20).
        /// Solo accesible para Administradores.
        /// POST: api/hospitals
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<HospitalViewModel>> Crear([FromBody] HospitalCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var hospital = await _servicio.CrearAsync(modelo);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = hospital.Id }, hospital);
            }
            catch (InvalidOperationException ex)
            {
                // El servicio lanza esta excepción cuando el RNC ya existe
                return Conflict(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos de un hospital existente (RF20).
        /// Solo accesible para Administradores.
        /// PUT: api/hospitals/5
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] HospitalActualizarViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _servicio.ActualizarAsync(id, modelo);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Hospital con id {id} no encontrado.");
            }
        }
    }
}
