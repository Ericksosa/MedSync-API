using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSync_API.Models
{
    /// <summary>
    /// Cita médica entre un paciente y un médico en un hospital (RF05, RF06, RF07).
    /// Punto central del sistema que conecta pacientes, médicos y pagos.
    /// </summary>
    [Table("Appointments")]
    public class Appointment
    {
        /// <summary>Identificador único de la cita.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Fecha y hora programada para la consulta.</summary>
        [Required]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Estado actual de la cita (RF10).
        /// Valores válidos: "Pendiente", "Confirmada", "En Progreso", "Completada", "Cancelada", "No Presentó".
        /// La validación de valores permitidos se realiza en la capa de servicios.
        /// </summary>
        [Required, MaxLength(20)]
        public required string Status { get; set; }

        /// <summary>Notas clínicas o instrucciones previas a la consulta.</summary>
        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>Paciente que asiste a la cita.</summary>
        [Required]
        public int PatientId { get; set; }

        /// <summary>Médico que atiende la cita.</summary>
        [Required]
        public int DoctorId { get; set; }

        /// <summary>Hospital donde se realiza la consulta.</summary>
        [Required]
        public int HospitalId { get; set; }

        // ─── Propiedades de navegación ───────────────────────────────────────

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        [ForeignKey("HospitalId")]
        public Hospital? Hospital { get; set; }

        /// <summary>Pago asociado a esta cita (relación 1 a 1, RF16).</summary>
        public Payment? Payment { get; set; }
    }
}
