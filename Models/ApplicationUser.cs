using Microsoft.AspNetCore.Identity;

namespace MedSync_API.Models
{
    /// <summary>
    /// Usuario del sistema que extiende IdentityUser de ASP.NET Identity.
    /// Gestiona la autenticación y autorización de Pacientes, Médicos y Administradores (RF02).
    /// Las contraseñas son almacenadas con BCrypt mediante el PasswordHasher de Identity (RNF06).
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Nombre de pila del usuario registrado en el sistema.
        /// </summary>
        public required string Nombre { get; set; }

        /// <summary>
        /// Apellido del usuario registrado en el sistema.
        /// </summary>
        public required string Apellido { get; set; }
    }
}
