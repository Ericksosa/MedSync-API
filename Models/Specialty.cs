using MedSync_API;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedSync_API.Models
{
    [Table("Specialties")]
    public class Specialty
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int HospitalId { get; set; }

        // Propiedades de navegación
        [ForeignKey("HospitalId")]
        public Hospital? Hospital { get; set; }

        [JsonIgnore] public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}