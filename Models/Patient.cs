using MedSync_API;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedSync_API.Models
{
    [Table("Patients")]
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public required string DocumentId { get; set; } // Cédula o Pasaporte

        [Required, MaxLength(50)]
        public required string FirstName { get; set; }

        [Required, MaxLength(50)]
        public required string LastName { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public DateTime DateOfBirth { get; set; }

        // Propiedades de navegación
        [JsonIgnore] public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}