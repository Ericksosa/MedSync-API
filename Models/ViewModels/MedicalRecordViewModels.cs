using System.ComponentModel.DataAnnotations;

namespace MedSync_API.Models.ViewModels
{
    /// <summary>
    /// ViewModel de respuesta con el expediente médico completo del paciente (RF14).
    /// Incluye diagnósticos y recetas para una vista consolidada.
    /// </summary>
    public class ExpedienteViewModel
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public string NombrePaciente { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }

        /// <summary>Diagnósticos registrados en este expediente (RF12).</summary>
        public IEnumerable<DiagnosticoViewModel> Diagnosticos { get; set; } = new List<DiagnosticoViewModel>();

        /// <summary>Recetas emitidas en este expediente (RF13, RF15).</summary>
        public IEnumerable<RecetaViewModel> Recetas { get; set; } = new List<RecetaViewModel>();
    }

    /// <summary>
    /// ViewModel de respuesta con los datos de un diagnóstico (RF12).
    /// </summary>
    public class DiagnosticoViewModel
    {
        public int Id { get; set; }
        public int ExpedienteId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int MedicoId { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public int? CitaId { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para registrar un diagnóstico en el expediente (RF12).
    /// </summary>
    public class DiagnosticoCrearViewModel
    {
        [Required(ErrorMessage = "El expediente es obligatorio.")]
        public int ExpedienteId { get; set; }

        [Required(ErrorMessage = "La descripción del diagnóstico es obligatoria.")]
        [MaxLength(1000)]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = "El médico es obligatorio.")]
        public int MedicoId { get; set; }

        /// <summary>Cita durante la cual se registró el diagnóstico (opcional).</summary>
        public int? CitaId { get; set; }
    }

    /// <summary>
    /// ViewModel de respuesta con los datos de una receta médica (RF13, RF15).
    /// </summary>
    public class RecetaViewModel
    {
        public int Id { get; set; }
        public int ExpedienteId { get; set; }
        public string Medicamento { get; set; } = string.Empty;
        public string Dosis { get; set; } = string.Empty;
        public string? Instrucciones { get; set; }
        public int MedicoId { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public int? CitaId { get; set; }
        public DateTime FechaEmision { get; set; }
        public bool EstaActiva { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para emitir una receta médica (RF13).
    /// </summary>
    public class RecetaCrearViewModel
    {
        [Required(ErrorMessage = "El expediente es obligatorio.")]
        public int ExpedienteId { get; set; }

        [Required(ErrorMessage = "El nombre del medicamento es obligatorio.")]
        [MaxLength(200)]
        public required string Medicamento { get; set; }

        [Required(ErrorMessage = "La dosis es obligatoria.")]
        [MaxLength(100)]
        public required string Dosis { get; set; }

        [MaxLength(500)]
        public string? Instrucciones { get; set; }

        [Required(ErrorMessage = "El médico es obligatorio.")]
        public int MedicoId { get; set; }

        /// <summary>Cita durante la cual se emitió la receta (opcional).</summary>
        public int? CitaId { get; set; }
    }

    /// <summary>
    /// ViewModel de respuesta con la disponibilidad horaria de un médico (RF23).
    /// </summary>
    public class DisponibilidadViewModel
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public string NombreMedico { get; set; } = string.Empty;

        /// <summary>Día de la semana: 0 = Lunes, 6 = Domingo.</summary>
        public int DiaSemana { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para configurar disponibilidad de un médico (RF23).
    /// </summary>
    public class DisponibilidadCrearViewModel
    {
        [Required(ErrorMessage = "El médico es obligatorio.")]
        public int MedicoId { get; set; }

        /// <summary>Día de la semana (0 = Lunes, 6 = Domingo).</summary>
        [Required]
        [Range(0, 6, ErrorMessage = "El día de la semana debe estar entre 0 (Lunes) y 6 (Domingo).")]
        public int DiaSemana { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        public TimeOnly HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        public TimeOnly HoraFin { get; set; }
    }
}
