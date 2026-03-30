using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedSync_API.Models;
using MedSync_API.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de autenticación.
    /// Gestiona el registro e inicio de sesión de usuarios mediante ASP.NET Identity y JWT (RF02, RNF04, RNF06).
    /// Las contraseñas son almacenadas con BCrypt mediante el PasswordHasher de Identity.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config)
        {
            _userManager   = userManager;
            _signInManager = signInManager;
            _config        = config;
        }

        /// <inheritdoc/>
        public async Task<TokenRespuestaViewModel?> LoginAsync(LoginViewModel modelo)
        {
            // Busca al usuario por correo electrónico en la tabla de Identity
            var usuario = await _userManager.FindByEmailAsync(modelo.Email);
            if (usuario is null) return null;

            // Verifica la contraseña usando el PasswordHasher de Identity (BCrypt)
            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, modelo.Contrasena, lockoutOnFailure: false);
            if (!resultado.Succeeded) return null;

            // Obtiene el rol del usuario para incluirlo en el token JWT
            var roles = await _userManager.GetRolesAsync(usuario);
            var rol   = roles.FirstOrDefault() ?? string.Empty;

            return GenerarToken(usuario, rol);
        }

        /// <inheritdoc/>
        public async Task<TokenRespuestaViewModel> RegistrarAsync(RegistroViewModel modelo)
        {
            // Valida que el rol indicado sea uno de los permitidos en el sistema
            string[] rolesValidos = ["Paciente", "Doctor", "Administrador"];
            if (!rolesValidos.Contains(modelo.Rol))
                throw new InvalidOperationException(
                    $"Rol '{modelo.Rol}' no válido. Use: {string.Join(", ", rolesValidos)}.");

            var usuario = new ApplicationUser
            {
                UserName = modelo.Email,
                Email    = modelo.Email,
                Nombre   = modelo.Nombre,
                Apellido = modelo.Apellido
            };

            // Identity hashea la contraseña internamente con BCrypt (RNF06)
            var resultado = await _userManager.CreateAsync(usuario, modelo.Contrasena);
            if (!resultado.Succeeded)
            {
                var errores = string.Join("; ", resultado.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Error al registrar el usuario: {errores}");
            }

            // Asigna el rol al usuario en la tabla AspNetUserRoles
            await _userManager.AddToRoleAsync(usuario, modelo.Rol);

            return GenerarToken(usuario, modelo.Rol);
        }

        // ─── Generación del token JWT ─────────────────────────────────────────

        /// <summary>
        /// Genera un token JWT firmado con las claims del usuario autenticado.
        /// El token incluye el email, nombre y rol para autorización en cada endpoint (RNF04).
        /// </summary>
        private TokenRespuestaViewModel GenerarToken(ApplicationUser usuario, string rol)
        {
            // Clave secreta obtenida de la configuración (debe sobrescribirse en .env o variables de entorno)
            var secret   = _config["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret no configurado.");
            var clave    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            // Claims que se incluyen en el payload del token
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id),
                new(ClaimTypes.Email, usuario.Email ?? string.Empty),
                new(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new(ClaimTypes.Role, rol)
            };

            var expiracionHoras = int.Parse(_config["JWT:ExpiracionHoras"] ?? "24");
            var expiracion      = DateTime.UtcNow.AddHours(expiracionHoras);

            var token = new JwtSecurityToken(
                issuer:   _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims:   claims,
                expires:  expiracion,
                signingCredentials: credenciales
            );

            return new TokenRespuestaViewModel
            {
                Token      = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion,
                Email      = usuario.Email ?? string.Empty,
                Rol        = rol
            };
        }
    }
}
