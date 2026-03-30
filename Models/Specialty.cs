using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedSync_API.Models
{
    /// <summary>
    /// Especialidad médica disponible en un hospital (RF21).
    /// Agrupa a los médicos que ejercen esa rama de la medicina.
    /// </summary>
    [Table("Specialties")]
    public class Specialty
    {
        /// <summary>Identificador único de la especialidad.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Nombre de la especialidad médica (ej: "Cardiología", "Pediatría").</summary>
        [Required, MaxLength(100)]
        public required string Name { get; set; }

        /// <summary>Descripción de los servicios o alcance de la especialidad.</summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>Hospital al que pertenece esta especialidad.</summary>
        [Required]
        public int HospitalId { get; set; }

        // ─── Propiedades de navegación ───────────────────────────────────────

        /// <summary>Hospital que ofrece esta especialidad.</summary>
        [ForeignKey("HospitalId")]
        public Hospital? Hospital { get; set; }

        /// <summary>Médicos que ejercen esta especialidad.</summary>
        [JsonIgnore] public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
