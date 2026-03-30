using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador de especialidades médicas (RF21).
    /// Gestiona las especialidades disponibles en cada hospital.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ISpecialtyService _servicio;

        public SpecialtiesController(ISpecialtyService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Obtiene las especialidades médicas de un hospital específico.
        /// GET: api/specialties/hospital/1
        /// </summary>
        [HttpGet("hospital/{hospitalId:int}")]
        public async Task<ActionResult<IEnumerable<EspecialidadViewModel>>> ObtenerPorHospital(int hospitalId)
        {
            var especialidades = await _servicio.ObtenerPorHospitalAsync(hospitalId);
            return Ok(especialidades);
        }

        /// <summary>
        /// Crea una nueva especialidad médica en un hospital (RF21).
        /// Solo accesible para Administradores.
        /// POST: api/specialties
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<EspecialidadViewModel>> Crear([FromBody] EspecialidadCrearViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var especialidad = await _servicio.CrearAsync(modelo);
                return Ok(especialidad);
            }
            catch (InvalidOperationException ex)
            {
                // Ya existe esa especialidad en el hospital indicado
                return Conflict(new { mensaje = ex.Message });
            }
        }
    }
}
