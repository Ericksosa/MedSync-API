using MedSync_API.Models.ViewModels;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        private readonly IPatientService _patientService;

        public AuthController(IAuthService servicio, IPatientService patientService)
        {
            _servicio = servicio;
            _patientService = patientService;
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
        /// El rol Paciente permite auto-registro público; los demás roles requieren Administrador o SuperAdmin.
        /// POST: api/auth/registro
        /// </summary>
        [HttpPost("registro")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenRespuestaViewModel>> Registro([FromBody] RegistroViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var esRegistroPaciente = string.Equals(modelo.Rol, "Paciente", StringComparison.OrdinalIgnoreCase);
            if (!esRegistroPaciente)
            {
                if (User?.Identity?.IsAuthenticated != true)
                    return Unauthorized(new { mensaje = "Debe iniciar sesión para crear este tipo de usuario." });

                if (!User.IsInRole("Administrador") && !User.IsInRole("SuperAdmin"))
                    return Forbid();
            }

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

        /// <summary>
        /// Registra una cuenta de paciente junto con su perfil clínico en una sola operación.
        /// POST: api/auth/registro-paciente
        /// </summary>
        [HttpPost("registro-paciente")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenRespuestaViewModel>> RegistroPaciente([FromBody] PacienteRegistroRequestViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _patientService.CrearAsync(new PacienteCrearViewModel
                {
                    DocumentoId = modelo.DocumentoId,
                    Nombre = modelo.Nombre,
                    Apellido = modelo.Apellido,
                    Email = modelo.Email,
                    Telefono = modelo.Telefono,
                    FechaNacimiento = modelo.FechaNacimiento
                });

                var token = await _servicio.RegistrarAsync(new RegistroViewModel
                {
                    Email = modelo.Email,
                    Contrasena = modelo.Contrasena,
                    Nombre = modelo.Nombre,
                    Apellido = modelo.Apellido,
                    Rol = "Paciente"
                });

                return Ok(token);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Lista usuarios del sistema, con filtro opcional por rol.
        /// GET: api/auth/users?rol=Administrador
        /// </summary>
        [HttpGet("users")]
        [Authorize(Roles = "SuperAdmin,Administrador")]
        public async Task<ActionResult<IEnumerable<UsuarioViewModel>>> ObtenerUsuarios([FromQuery] string? rol)
        {
            var usuarios = await _servicio.ObtenerUsuariosAsync(rol);
            return Ok(usuarios);
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado.
        /// GET: api/auth/me
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<PerfilUsuarioViewModel>> ObtenerMiPerfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var perfil = await _servicio.ObtenerPerfilAsync(userId);
            if (perfil is null) return NotFound(new { mensaje = "Usuario no encontrado." });
            return Ok(perfil);
        }

        /// <summary>
        /// Actualiza nombre y correo del usuario autenticado.
        /// PUT: api/auth/me
        /// </summary>
        [HttpPut("me")]
        [Authorize]
        public async Task<ActionResult<TokenRespuestaViewModel>> ActualizarMiPerfil([FromBody] PerfilActualizarViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            try
            {
                var token = await _servicio.ActualizarPerfilAsync(userId, modelo);
                return Ok(token);
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
        /// Cambia la contraseña del usuario autenticado.
        /// POST: api/auth/me/cambiar-contrasena
        /// </summary>
        [HttpPost("me/cambiar-contrasena")]
        [Authorize]
        public async Task<IActionResult> CambiarMiContrasena([FromBody] CambiarContrasenaViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            try
            {
                await _servicio.CambiarContrasenaAsync(userId, modelo);
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
    }
}
