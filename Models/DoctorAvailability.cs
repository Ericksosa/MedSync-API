using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSync_API.Models
{
    /// <summary>
    /// Disponibilidad horaria de un médico en días específicos de la semana (RF23).
    /// Define las franjas en que el médico puede recibir citas.
    /// </summary>
    [Table("DoctorAvailabilities")]
    public class DoctorAvailability
    {
        /// <summary>Identificador único del registro de disponibilidad.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Médico al que pertenece esta franja de disponibilidad.</summary>
        [Required]
        public int DoctorId { get; set; }

        /// <summary>Hospital al que aplica esta disponibilidad del médico.</summary>
        public int? HospitalId { get; set; }

        /// <summary>
        /// Día de la semana (0 = Lunes, 1 = Martes, ..., 6 = Domingo).
        /// Se usa entero para facilitar comparaciones y consultas.
        /// </summary>
        [Required, Range(0, 6)]
        public int DayOfWeek { get; set; }

        /// <summary>Hora de inicio de la franja disponible (ej: 08:00).</summary>
        [Required]
        public TimeOnly StartTime { get; set; }

        /// <summary>Hora de fin de la franja disponible (ej: 17:00).</summary>
        [Required]
        public TimeOnly EndTime { get; set; }

        // ─── Propiedades de navegación ───────────────────────────────────────

        /// <summary>Médico al que corresponde esta disponibilidad.</summary>
        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        /// <summary>Hospital asociado a la disponibilidad.</summary>
        [ForeignKey("HospitalId")]
        public Hospital? Hospital { get; set; }
    }
}
