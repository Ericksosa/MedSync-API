using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSync_API.Models
{
    /// <summary>
    /// Pago registrado por la consulta médica de un paciente (RF16).
    /// Tiene una relación 1 a 1 con la cita correspondiente.
    /// </summary>
    [Table("Payments")]
    public class Payment
    {
        /// <summary>Identificador único del pago.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Cita médica que origina este pago (relación 1 a 1).</summary>
        [Required]
        public int AppointmentId { get; set; }

        /// <summary>Monto cobrado al paciente por la consulta.</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Amount { get; set; }

        /// <summary>
        /// Método de pago utilizado.
        /// Valores válidos: "Card", "Cash", "Insurance".
        /// </summary>
        [Required, MaxLength(50)]
        public required string PaymentMethod { get; set; }

        /// <summary>
        /// Estado actual del pago.
        /// Valores válidos: "Completed", "Pending", "Refunded".
        /// </summary>
        [Required, MaxLength(20)]
        public required string Status { get; set; }

        /// <summary>Fecha y hora en que se registró el pago (UTC).</summary>
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // ─── Propiedades de navegación ───────────────────────────────────────

        /// <summary>Cita asociada a este pago.</summary>
        [ForeignKey("AppointmentId")]
        public Appointment? Appointment { get; set; }
    }
}
