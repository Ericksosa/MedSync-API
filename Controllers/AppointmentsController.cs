using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador de citas médicas (RF05, RF06, RF07, RF08, RF09, RF10).
    /// Gestiona la reserva, consulta y cambio de estado de las citas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _servicio;

        public AppointmentsController(IAppointmentService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Obtiene todas las citas de un hospital para gestión administrativa.
        /// GET: api/appointments/hospital/1
        /// </summary>
        [HttpGet("hospital/{hospitalId:int}")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<IEnumerable<CitaViewModel>>> ObtenerPorHospital(int hospitalId)
        {
            var citas = await _servicio.ObtenerPorHospitalAsync(hospitalId);
            return Ok(citas);
        }

        /// <summary>
        /// Obtiene el historial completo de citas de un paciente (RF09).
        /// GET: api/appointments/paciente/5
        /// </summary>
        [HttpGet("paciente/{pacienteId:int}")]
        public async Task<ActionResult<IEnumerable<CitaViewModel>>> ObtenerPorPaciente(int pacienteId)
        {
            var citas = await _servicio.ObtenerPorPacienteAsync(pacienteId);
            return Ok(citas);
        }

        /// <summary>
        /// Obtiene la agenda de un médico en una fecha específica (RF08).
        /// GET: api/appointments/medico/3/agenda?fecha=2026-03-29
        /// </summary>
        [HttpGet("medico/{medicoId:int}/agenda")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<IEnumerable<CitaViewModel>>> ObtenerAgendaMedico(
            int medicoId, [FromQuery] DateTime fecha)
        {
            var citas = await _servicio.ObtenerAgendaMedicoAsync(medicoId, fecha);
            return Ok(citas);
        }

        /// <summary>
        /// Obtiene una cita por su identificador.
        /// GET: api/appointments/5
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CitaViewModel>> ObtenerPorId(int id)
        {
            var cita = await _servicio.ObtenerPorIdAsync(id);
            if (cita is null) return NotFound($"Cita con id {id} no encontrada.");
            return Ok(cita);
        }

        /// <summary>
        /// Reserva una nueva cita médica para un paciente (RF05).
        /// Valida disponibilidad del médico y ausencia de conflictos de horario.
        /// POST: api/appointments
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CitaViewModel>> Crear([FromBody] CitaCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var cita = await _servicio.CrearAsync(modelo);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = cita.Id }, cita);
            }
            catch (InvalidOperationException ex)
            {
                // Conflicto de horario o fecha en el pasado
                return Conflict(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estado de una cita (RF07, RF10).
        /// Valores válidos: "Pendiente", "Confirmada", "En Progreso", "Completada", "Cancelada", "No Presentó".
        /// PATCH: api/appointments/5/estado
        /// </summary>
        [HttpPatch("{id:int}/estado")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CitaEstadoViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _servicio.CambiarEstadoAsync(id, modelo.Estado);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Cita con id {id} no encontrada.");
            }
            catch (InvalidOperationException ex)
            {
                // Estado inválido o transición no permitida
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
