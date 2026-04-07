using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador de cobros de consultorio (RF17, RF18).
    /// </summary>
    [Route("api/cobros")]
    [ApiController]
    [Authorize]
    public class CobrosController : ControllerBase
    {
        private readonly ICobroService _servicio;

        public CobrosController(ICobroService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Obtiene cobros por hospital.
        /// GET: api/cobros/hospital/1
        /// </summary>
        [HttpGet("hospital/{hospitalId:int}")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<IEnumerable<CobroViewModel>>> ObtenerPorHospital(int hospitalId)
        {
            var cobros = await _servicio.ObtenerPorHospitalAsync(hospitalId);
            return Ok(cobros);
        }

        /// <summary>
        /// Registra un nuevo cobro de consultorio.
        /// POST: api/cobros
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<CobroViewModel>> Crear([FromBody] CobroCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var cobro = await _servicio.CrearAsync(modelo);
                return Ok(cobro);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estado de un cobro.
        /// PATCH: api/cobros/5/estado
        /// </summary>
        [HttpPatch("{id:int}/estado")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CobroEstadoViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _servicio.CambiarEstadoAsync(id, modelo.Estado);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el balance financiero de un médico.
        /// GET: api/cobros/doctor/3/balance
        /// </summary>
        [HttpGet("doctor/{doctorId:int}/balance")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<BalanceMedicoViewModel>> ObtenerBalanceDoctor(int doctorId)
        {
            try
            {
                var balance = await _servicio.ObtenerBalanceDoctorAsync(doctorId);
                return Ok(balance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}