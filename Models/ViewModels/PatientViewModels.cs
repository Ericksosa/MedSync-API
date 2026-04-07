using System.ComponentModel.DataAnnotations;

namespace MedSync_API.Models.ViewModels
{
    /// <summary>
    /// ViewModel de respuesta con los datos del paciente.
    /// </summary>
    public class PacienteViewModel
    {
        public int Id { get; set; }
        public string DocumentoId { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para registrar un nuevo paciente (RF01).
    /// Al registrarse, el servicio crea automáticamente el expediente médico (RF11).
    /// </summary>
    public class PacienteCrearViewModel
    {
        /// <summary>Cédula de identidad o número de pasaporte.</summary>
        [Required(ErrorMessage = "El documento de identidad es obligatorio.")]
        [MaxLength(20)]
        public required string DocumentoId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50)]
        public required string Apellido { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime FechaNacimiento { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para actualizar los datos de un paciente (RF04).
    /// </summary>
    public class PacienteActualizarViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50)]
        public required string Apellido { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public DateTime? FechaNacimiento { get; set; }
    }
}
