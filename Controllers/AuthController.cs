using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedSync_API.Controllers
{
    /// <summary>
    /// Controlador de autenticación y gestión de usuarios (RF02, RNF04, RNF06).
    /// Gestiona el inicio de sesión y el registro de nuevos usuarios en el sistema.
    /// Las contraseñas son almacenadas con BCrypt mediante ASP.NET Identity.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _servicio;

        public AuthController(IAuthService servicio)
        {
            _servicio = servicio;
        }

        /// <summary>
        /// Autentica al usuario y retorna un token JWT si las credenciales son válidas (RF02).
        /// El token debe incluirse en el encabezado Authorization de las solicitudes protegidas.
        /// POST: api/auth/login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous] // Este endpoint es público: no requiere token previo
        public async Task<ActionResult<TokenRespuestaViewModel>> Login([FromBody] LoginViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var token = await _servicio.LoginAsync(modelo);
            if (token is null)
                return Unauthorized(new { mensaje = "Correo electrónico o contraseña incorrectos." });

            return Ok(token);
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema con el rol indicado (RF02, RF22).
        /// Solo accesible para Administradores: son quienes crean cuentas de médicos y administradores.
        /// POST: api/auth/registro
        /// </summary>
        [HttpPost("registro")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<TokenRespuestaViewModel>> Registro([FromBody] RegistroViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var token = await _servicio.RegistrarAsync(modelo);
                return Ok(token);
            }
            catch (InvalidOperationException ex)
            {
                // Rol inválido o errores de validación de Identity (contraseña débil, email duplicado, etc.)
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
