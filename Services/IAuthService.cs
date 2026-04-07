using MedSync_API.Models.ViewModels;

namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de autenticación.
    /// Gestiona el inicio de sesión y el registro de usuarios en el sistema (RF02, RNF04, RNF06).
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Autentica al usuario con su correo y contraseña.
        /// Retorna un token JWT si las credenciales son válidas (RF02, RNF04).
        /// </summary>
        Task<TokenRespuestaViewModel?> LoginAsync(LoginViewModel modelo);

        /// <summary>
        /// Registra un nuevo usuario en el sistema con el rol indicado.
        /// Usado por el administrador para crear cuentas de médicos o administradores (RF02).
        /// </summary>
        Task<TokenRespuestaViewModel> RegistrarAsync(RegistroViewModel modelo);

        /// <summary>
        /// Lista usuarios del sistema con filtro opcional por rol.
        /// </summary>
        Task<IEnumerable<UsuarioViewModel>> ObtenerUsuariosAsync(string? rol = null);

        /// <summary>Obtiene el perfil del usuario autenticado.</summary>
        Task<PerfilUsuarioViewModel?> ObtenerPerfilAsync(string userId);

        /// <summary>Actualiza nombre y correo del usuario autenticado.</summary>
        Task<TokenRespuestaViewModel> ActualizarPerfilAsync(string userId, PerfilActualizarViewModel modelo);

        /// <summary>Cambia la contraseña del usuario autenticado.</summary>
        Task CambiarContrasenaAsync(string userId, CambiarContrasenaViewModel modelo);
    }
}
