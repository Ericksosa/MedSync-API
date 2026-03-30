using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador del expediente médico electrónico (RF11-RF15).
    /// Gestiona la consulta del expediente, registro de diagnósticos y emisión de recetas.
    /// Solo médicos y administradores del hospital pueden acceder (RNF07).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Doctor")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _servicio;

        public MedicalRecordsController(IMedicalRecordService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Obtiene el expediente médico completo de un paciente con diagnósticos y recetas (RF14).
        /// GET: api/medicalrecords/paciente/5
        /// </summary>
        [HttpGet("paciente/{pacienteId:int}")]
        public async Task<ActionResult<ExpedienteViewModel>> ObtenerPorPaciente(int pacienteId)
        {
            var expediente = await _servicio.ObtenerPorPacienteAsync(pacienteId);
            if (expediente is null)
                return NotFound($"No se encontró un expediente médico para el paciente con id {pacienteId}.");
            return Ok(expediente);
        }

        /// <summary>
        /// Registra un nuevo diagnóstico en el expediente de un paciente (RF12).
        /// POST: api/medicalrecords/diagnosticos
        /// </summary>
        [HttpPost("diagnosticos")]
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
    }
}
