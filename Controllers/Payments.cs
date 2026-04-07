using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador de pagos de consultas médicas (RF16, RF19).
    /// Gestiona el registro y consulta de pagos por citas realizadas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _servicio;

        public PaymentsController(IPaymentService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Obtiene todos los pagos de un hospital para auditoría financiera (RF19).
        /// GET: api/payments/hospital/1
        /// </summary>
        [HttpGet("hospital/{hospitalId:int}")]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<IEnumerable<PagoViewModel>>> ObtenerPorHospital(int hospitalId)
        {
            var pagos = await _servicio.ObtenerPorHospitalAsync(hospitalId);
            return Ok(pagos);
        }

        /// <summary>
        /// Registra el pago de una consulta médica (RF16).
        /// POST: api/payments
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador,Doctor")]
        public async Task<ActionResult<PagoViewModel>> Crear([FromBody] PagoCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var pago = await _servicio.CrearAsync(modelo);
                return Ok(pago);
            }
            catch (InvalidOperationException ex)
            {
                // La cita ya tiene pago registrado, o método/estado inválido
                return Conflict(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el estado de un pago registrado.
        /// PATCH: api/payments/5/estado
        /// </summary>
        [HttpPatch("{id:int}/estado")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] PagoEstadoViewModel modelo)
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
        /// Elimina un pago registrado por error.
        /// Solo se permite eliminar pagos en estado "Pending" (RF16).
        /// DELETE: api/payments/5
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _servicio.EliminarAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Pago con id {id} no encontrado.");
            }
            catch (InvalidOperationException ex)
            {
                // No se puede eliminar un pago completado
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
