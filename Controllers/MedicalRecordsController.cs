using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador del expediente médico electrónico (RF11-RF15).
    /// Gestiona la consulta del expediente, registro de diagnósticos y emisión de recetas.
    /// Solo médicos y administradores del hospital pueden acceder (RNF07).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _servicio;
        private readonly IPatientService _pacienteServicio;

        public MedicalRecordsController(IMedicalRecordService servicio, IPatientService pacienteServicio)
        {
            _servicio = servicio;
            _pacienteServicio = pacienteServicio;
        }

        /// <summary>
        /// Obtiene el expediente médico completo de un paciente con diagnósticos y recetas (RF14).
        /// GET: api/medicalrecords/paciente/5
        /// </summary>
        [HttpGet("paciente/{pacienteId:int}")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<ExpedienteViewModel>> ObtenerPorPaciente(int pacienteId)
        {
            var expediente = await _servicio.ObtenerPorPacienteAsync(pacienteId);
            if (expediente is null)
                return NotFound($"No se encontró un expediente médico para el paciente con id {pacienteId}.");
            return Ok(expediente);
        }

        /// <summary>
        /// Obtiene el expediente médico del paciente autenticado.
        /// GET: api/medicalrecords/me
        /// </summary>
        [HttpGet("me")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult<ExpedienteViewModel>> ObtenerMiExpediente()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var paciente = await _pacienteServicio.ObtenerPorEmailAsync(email);
            if (paciente is null)
                return NotFound("No se encontró un paciente asociado al usuario autenticado.");

            var expediente = await _servicio.ObtenerPorPacienteAsync(paciente.Id);
            if (expediente is null)
                return NotFound("No se encontró tu expediente médico.");

            return Ok(expediente);
        }

        /// <summary>
        /// Registra un nuevo diagnóstico en el expediente de un paciente (RF12).
        /// POST: api/medicalrecords/diagnosticos
        /// </summary>
        [HttpPost("diagnosticos")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<DiagnosticoViewModel>> RegistrarDiagnostico(
            [FromBody] DiagnosticoCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var diagnostico = await _servicio.RegistrarDiagnosticoAsync(modelo);
                return Ok(diagnostico);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Emite una receta médica y la vincula al expediente del paciente (RF13).
        /// POST: api/medicalrecords/recetas
        /// </summary>
        [HttpPost("recetas")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<RecetaViewModel>> EmitirReceta(
            [FromBody] RecetaCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var receta = await _servicio.EmitirRecetaAsync(modelo);
                return Ok(receta);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los signos vitales del expediente de un paciente.
        /// PATCH: api/medicalrecords/5/signos-vitales
        /// </summary>
        [HttpPatch("{expedienteId:int}/signos-vitales")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<IActionResult> ActualizarSignosVitales(
            int expedienteId,
            [FromBody] SignosVitalesActualizarViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _servicio.ActualizarSignosVitalesAsync(expedienteId, modelo);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}
