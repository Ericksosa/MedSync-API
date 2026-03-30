using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador de pacientes (RF01, RF04, RF09).
    /// Gestiona el registro y actualización de pacientes.
    /// Al registrar un paciente, se crea automáticamente su expediente médico (RF11).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _servicio;

        public PatientsController(IPatientService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Busca un paciente por su documento de identidad (cédula o pasaporte).
        /// GET: api/patients/documento/40200000000
        /// </summary>
        [HttpGet("documento/{documentoId}")]
        public async Task<ActionResult<PacienteViewModel>> ObtenerPorDocumento(string documentoId)
        {
            var paciente = await _servicio.ObtenerPorDocumentoAsync(documentoId);
            if (paciente is null) return NotFound($"No se encontró un paciente con el documento '{documentoId}'.");
            return Ok(paciente);
        }

        /// <summary>
        /// Obtiene un paciente por su identificador interno.
        /// GET: api/patients/5
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PacienteViewModel>> ObtenerPorId(int id)
        {
            var paciente = await _servicio.ObtenerPorIdAsync(id);
            if (paciente is null) return NotFound($"Paciente con id {id} no encontrado.");
            return Ok(paciente);
        }

        /// <summary>
        /// Registra un nuevo paciente y crea su expediente médico automáticamente (RF01, RF11).
        /// POST: api/patients
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<PacienteViewModel>> Crear([FromBody] PacienteCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var paciente = await _servicio.CrearAsync(modelo);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = paciente.Id }, paciente);
            }
            catch (InvalidOperationException ex)
            {
                // El servicio lanza esta excepción cuando el documento de identidad ya existe
                return Conflict(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos personales de un paciente (RF04).
        /// PUT: api/patients/5
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] PacienteActualizarViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _servicio.ActualizarAsync(id, modelo);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Paciente con id {id} no encontrado.");
            }
        }
    }
}
