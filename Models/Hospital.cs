using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedSync_API.Models
{
    /// <summary>
    /// Centro médico u hospital registrado en el sistema (RF20).
    /// Es la entidad raíz del dominio: agrupa especialidades, médicos, citas y cobros.
    /// </summary>
    [Table("Hospitals")]
    public class Hospital
    {
        /// <summary>Identificador único del hospital.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Nombre comercial del centro médico.</summary>
        [Required, MaxLength(100)]
        public required string Name { get; set; }

        /// <summary>Registro Nacional del Contribuyente (RNC) del hospital.</summary>
        [Required, MaxLength(20)]
        public required string Rnc { get; set; }

        /// <summary>Dirección física del hospital.</summary>
        [MaxLength(255)]
        public string? Address { get; set; }

        /// <summary>Número de teléfono principal del hospital.</summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>Indica si el hospital está operativo en el sistema (RF20).</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Fecha y hora en que fue registrado el hospital en el sistema (UTC).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ─── Propiedades de navegación ───────────────────────────────────────

        /// <summary>Especialidades médicas disponibles en este hospital (RF21).</summary>
        [JsonIgnore] public ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();

        /// <summary>Médicos que ejercen en este hospital (RF22).</summary>
        [JsonIgnore] public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

        /// <summary>Asignaciones de médicos a este hospital (muchos-a-muchos).</summary>
        [JsonIgnore] public ICollection<DoctorHospital> DoctorHospitals { get; set; } = new List<DoctorHospital>();

        /// <summary>Citas realizadas en este hospital.</summary>
        [JsonIgnore] public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        /// <summary>Cobros de consultorio emitidos en este hospital (RF17).</summary>
        [JsonIgnore] public ICollection<DoctorOfficeRent> OfficeRents { get; set; } = new List<DoctorOfficeRent>();
    }
}
