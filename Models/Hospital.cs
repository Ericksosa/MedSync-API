using MedSync_API;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedSync_API.Models
{
    [Table("Hospitals")]
    public class Hospital
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [Required, MaxLength(20)]
        public required string Rnc { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        [JsonIgnore] public ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
        [JsonIgnore] public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        [JsonIgnore] public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}