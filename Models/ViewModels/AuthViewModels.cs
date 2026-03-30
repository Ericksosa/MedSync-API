using System.ComponentModel.DataAnnotations;

namespace MedSync_API.Models.ViewModels
{
    /// <summary>
    /// ViewModel de entrada para iniciar sesión en el sistema (RF02).
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public required string Contrasena { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para registrar un nuevo usuario en el sistema.
    /// Usado por el administrador para crear cuentas de médicos y administradores (RF02, RF22).
    /// </summary>
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public required string Contrasena { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50)]
        public required string Apellido { get; set; }

        /// <summary>
        /// Rol asignado al usuario.
        /// Valores válidos: "Paciente", "Doctor", "Administrador".
        /// </summary>
        [Required(ErrorMessage = "El rol es obligatorio.")]
        public required string Rol { get; set; }
    }

    /// <summary>
    /// ViewModel de respuesta tras un inicio de sesión o registro exitoso.
    /// Contiene el token JWT para usar en las solicitudes protegidas (RNF04).
    /// </summary>
    public class TokenRespuestaViewModel
    {
        /// <summary>Token JWT firmado que el cliente debe enviar en el encabezado Authorization.</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Fecha y hora de expiración del token (UTC).</summary>
        public DateTime Expiracion { get; set; }

        /// <summary>Correo electrónico del usuario autenticado.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Rol del usuario (Paciente, Doctor o Administrador).</summary>
        public string Rol { get; set; } = string.Empty;
    }
}
