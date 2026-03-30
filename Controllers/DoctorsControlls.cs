using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador del directorio de médicos (RF22, RF23).
    /// Gestiona el registro, activación y disponibilidad horaria de los profesionales de salud.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Todas las rutas requieren JWT válido (RNF04)
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _servicio;

        public DoctorsController(IDoctorService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Obtiene los médicos de un hospital específico con su especialidad incluida.
        /// GET: api/doctors/hospital/1
        /// </summary>
        [HttpGet("hospital/{hospitalId:int}")]
        public async Task<ActionResult<IEnumerable<MedicoViewModel>>> ObtenerPorHospital(int hospitalId)
        {
            var medicos = await _servicio.ObtenerPorHospitalAsync(hospitalId);
            return Ok(medicos);
        }

        /// <summary>
        /// Obtiene los médicos activos de una especialidad en un hospital (usado al reservar cita, RF05).
        /// GET: api/doctors/hospital/1/especialidad/2
        /// </summary>
        [HttpGet("hospital/{hospitalId:int}/especialidad/{especialidadId:int}")]
        public async Task<ActionResult<IEnumerable<MedicoViewModel>>> ObtenerActivosPorEspecialidad(
            int hospitalId, int especialidadId)
        {
            var medicos = await _servicio.ObtenerActivosPorEspecialidadAsync(especialidadId, hospitalId);
            return Ok(medicos);
        }

        /// <summary>
        /// Obtiene un médico por su identificador.
        /// GET: api/doctors/5
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MedicoViewModel>> ObtenerPorId(int id)
        {
            var medico = await _servicio.ObtenerPorIdAsync(id);
            if (medico is null) return NotFound($"Médico con id {id} no encontrado.");
            return Ok(medico);
        }

        /// <summary>
        /// Registra un nuevo médico en el sistema (RF22).
        /// Solo accesible para Administradores.
        /// POST: api/doctors
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<MedicoViewModel>> Crear([FromBody] MedicoCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var medico = await _servicio.CrearAsync(modelo);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = medico.Id }, medico);
            }
            catch (InvalidOperationException ex)
            {
                // El servicio lanza esta excepción cuando el exequátur ya está registrado
                return Conflict(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva el perfil de un médico sin eliminarlo (RF22).
        /// Solo accesible para Administradores.
        /// PATCH: api/doctors/5/estado
        /// </summary>
        [HttpPatch("{id:int}/estado")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] MedicoEstadoViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _servicio.CambiarEstadoAsync(id, modelo.EstaActivo);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Médico con id {id} no encontrado.");
            }
        }

        /// <summary>
        /// Obtiene la disponibilidad horaria de un médico (RF23).
        /// GET: api/doctors/5/disponibilidad
        /// </summary>
        [HttpGet("{medicoId:int}/disponibilidad")]
        public async Task<ActionResult<IEnumerable<DisponibilidadViewModel>>> ObtenerDisponibilidad(int medicoId)
        {
            var disponibilidades = await _servicio.ObtenerDisponibilidadAsync(medicoId);
            return Ok(disponibilidades);
        }

        /// <summary>
        /// Configura una franja horaria de disponibilidad para un médico (RF23).
        /// Accesible para Administradores y el propio Doctor.
        /// POST: api/doctors/disponibilidad
        /// </summary>
        [HttpPost("disponibilidad")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<DisponibilidadViewModel>> AgregarDisponibilidad(
            [FromBody] DisponibilidadCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var disponibilidad = await _servicio.AgregarDisponibilidadAsync(modelo);
                return Ok(disponibilidad);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una franja de disponibilidad del médico (RF23).
        /// DELETE: api/doctors/disponibilidad/3
        /// </summary>
        [HttpDelete("disponibilidad/{id:int}")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<IActionResult> EliminarDisponibilidad(int id)
        {
            try
            {
                await _servicio.EliminarDisponibilidadAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Disponibilidad con id {id} no encontrada.");
            }
        }
    }
}
