using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedSync_API.Models
{
    /// <summary>
    /// Médico registrado en el sistema (RF22).
    /// Pertenece a un hospital y una especialidad.
    /// Puede ser activado o desactivado sin eliminar su historial.
    /// </summary>
    [Table("Doctors")]
    public class Doctor
    {
        /// <summary>Identificador único del médico.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Nombre de pila del médico.</summary>
        [Required, MaxLength(50)]
        public required string FirstName { get; set; }

        /// <summary>Apellido del médico.</summary>
        [Required, MaxLength(50)]
        public required string LastName { get; set; }

        /// <summary>Número de exequátur (licencia médica) emitido por el Ministerio de Salud.</summary>
        [Required, MaxLength(50)]
        public required string MedicalLicense { get; set; }

        /// <summary>Especialidad médica a la que pertenece el doctor.</summary>
        [Required]
        public int SpecialtyId { get; set; }

        /// <summary>Hospital donde ejerce el médico.</summary>
        [Required]
        public int HospitalId { get; set; }

        /// <summary>Indica si el perfil del médico está activo en el sistema (RF22).</summary>
        public bool IsActive { get; set; } = true;

        // ─── Propiedades de navegación ───────────────────────────────────────

        [ForeignKey("SpecialtyId")]
        public Specialty? Specialty { get; set; }

        [ForeignKey("HospitalId")]
        public Hospital? Hospital { get; set; }

        /// <summary>Hospitales donde el medico atiende (relacion muchos-a-muchos).</summary>
        [JsonIgnore] public ICollection<DoctorHospital> DoctorHospitals { get; set; } = new List<DoctorHospital>();

        /// <summary>Citas asignadas a este médico (RF08).</summary>
        [JsonIgnore] public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        /// <summary>Franjas horarias disponibles del médico (RF23).</summary>
        [JsonIgnore] public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();

        /// <summary>Cobros de consultorio facturados a este médico (RF17).</summary>
        [JsonIgnore] public ICollection<DoctorOfficeRent> OfficeRents { get; set; } = new List<DoctorOfficeRent>();
    }
}
