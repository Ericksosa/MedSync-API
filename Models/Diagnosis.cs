using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSync_API.Models
{
    /// <summary>
    /// Diagnóstico médico registrado por un médico en el expediente de un paciente (RF12).
    /// Puede estar vinculado a una cita específica.
    /// </summary>
    [Table("Diagnoses")]
    public class Diagnosis
    {
        /// <summary>Identificador único del diagnóstico.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Expediente médico al que pertenece este diagnóstico.</summary>
        [Required]
        public int MedicalRecordId { get; set; }

        /// <summary>Descripción clínica del diagnóstico emitido por el médico.</summary>
        [Required, MaxLength(1000)]
        public required string Description { get; set; }

        /// <summary>Médico que registró el diagnóstico.</summary>
        [Required]
        public int DoctorId { get; set; }

        /// <summary>Cita médica asociada a este diagnóstico (opcional).</summary>
        public int? AppointmentId { get; set; }

        /// <summary>Fecha y hora en que fue registrado el diagnóstico (UTC).</summary>
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // ─── Propiedades de navegación ───────────────────────────────────────

        /// <summary>Expediente al que pertenece este diagnóstico.</summary>
        [ForeignKey("MedicalRecordId")]
        public MedicalRecord? MedicalRecord { get; set; }

        /// <summary>Médico que emitió el diagnóstico.</summary>
        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        /// <summary>Cita vinculada al diagnóstico (puede ser nula).</summary>
        [ForeignKey("AppointmentId")]
        public Appointment? Appointment { get; set; }
    }
}
