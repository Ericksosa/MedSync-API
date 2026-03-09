using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSync_API.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Amount { get; set; }

        [Required, MaxLength(50)]
        public required string PaymentMethod { get; set; } // Card, Cash, Insurance

        [Required, MaxLength(20)]
        public required string Status { get; set; } // Completed, Pending, Refunded

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        [ForeignKey("AppointmentId")]
        public Appointment? Appointment { get; set; }
    }
}