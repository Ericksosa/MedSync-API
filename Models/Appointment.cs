using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSync_API.Models
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required, MaxLength(20)]
        public required string Status { get; set; } // Pending, Confirmed, Cancelled, Completed

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        // Propiedades de navegación
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        [ForeignKey("HospitalId")]
        public Hospital? Hospital { get; set; }

        // Relación 1 a 1 con el pago
        public Payment? Payment { get; set; }
    }
}