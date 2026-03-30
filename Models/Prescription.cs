using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSync_API.Models
{
    /// <summary>
    /// Receta médica emitida por un médico durante o tras una consulta (RF13).
    /// Queda registrada en el expediente del paciente y puede ser consultada por
    /// cualquier médico del mismo hospital (RF14, RF15).
    /// </summary>
    [Table("Prescriptions")]
    public class Prescription
    {
        /// <summary>Identificador único de la receta.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Expediente médico al que pertenece esta receta.</summary>
        [Required]
        public int MedicalRecordId { get; set; }

        /// <summary>Nombre del medicamento prescrito.</summary>
        [Required, MaxLength(200)]
        public required string Medication { get; set; }

        /// <summary>Dosis indicada (ej: "500mg cada 8 horas").</summary>
        [Required, MaxLength(100)]
        public required string Dosage { get; set; }

        /// <summary>Instrucciones adicionales de administración del medicamento.</summary>
        [MaxLength(500)]
        public string? Instructions { get; set; }

        /// <summary>Médico que emitió la receta.</summary>
        [Required]
        public int DoctorId { get; set; }

        /// <summary>Cita asociada a la emisión de esta receta (opcional).</summary>
        public int? AppointmentId { get; set; }

        /// <summary>Fecha y hora de emisión de la receta (UTC).</summary>
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Indica si la receta sigue vigente o ha sido reemplazada/expirada.</summary>
        public bool IsActive { get; set; } = true;

        // ─── Propiedades de navegación ───────────────────────────────────────

        /// <summary>Expediente al que pertenece esta receta.</summary>
        [ForeignKey("MedicalRecordId")]
        public MedicalRecord? MedicalRecord { get; set; }

        /// <summary>Médico que emitió la receta.</summary>
        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        /// <summary>Cita vinculada a la receta (puede ser nula).</summary>
        [ForeignKey("AppointmentId")]
        public Appointment? Appointment { get; set; }
    }
}
