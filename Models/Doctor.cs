using MedSync_API;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedSync_API.Models
{
    [Table("Doctors")]
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public required string FirstName { get; set; }

        [Required, MaxLength(50)]
        public required string LastName { get; set; }

        [Required, MaxLength(50)]
        public required string MedicalLicense { get; set; } // Exequátur

        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        public bool IsActive { get; set; } = true;

        // Propiedades de navegación
        [ForeignKey("SpecialtyId")]
        public Specialty? Specialty { get; set; }

        [ForeignKey("HospitalId")]
        public Hospital? Hospital { get; set; }

        [JsonIgnore] public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}