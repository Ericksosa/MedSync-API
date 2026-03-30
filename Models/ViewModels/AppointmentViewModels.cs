using System.ComponentModel.DataAnnotations;

namespace MedSync_API.Models.ViewModels
{
    /// <summary>
    /// ViewModel de respuesta con los datos completos de una cita.
    /// Incluye nombres de paciente, médico y hospital para facilitar la presentación.
    /// </summary>
    public class CitaViewModel
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Notas { get; set; }
        public int PacienteId { get; set; }
        public string NombrePaciente { get; set; } = string.Empty;
        public int MedicoId { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public int HospitalId { get; set; }
        public string NombreHospital { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel de entrada para reservar una nueva cita (RF05).
    /// El estado inicial siempre es "Pendiente" y lo asigna el servicio.
    /// </summary>
    public class CitaCrearViewModel
    {
        [Required(ErrorMessage = "La fecha y hora de la cita son obligatorias.")]
        public DateTime FechaHora { get; set; }

        [MaxLength(500)]
        public string? Notas { get; set; }

        [Required(ErrorMessage = "El paciente es obligatorio.")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "El médico es obligatorio.")]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "El hospital es obligatorio.")]
        public int HospitalId { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para cambiar el estado de una cita (RF07, RF10).
    /// Valores válidos: "Pendiente", "Confirmada", "En Progreso", "Completada", "Cancelada", "No Presentó".
    /// </summary>
    public class CitaEstadoViewModel
    {
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(20)]
        public required string Estado { get; set; }
    }
}
