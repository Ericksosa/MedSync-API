using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedSync_API.Models
{
    /// <summary>
    /// Paciente registrado en el sistema (RF01).
    /// Al crearse, genera automáticamente su expediente médico electrónico (RF11).
    /// </summary>
    [Table("Patients")]
    public class Patient
    {
        /// <summary>Identificador único del paciente.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Cédula de identidad o número de pasaporte del paciente.</summary>
        [Required, MaxLength(20)]
        public required string DocumentId { get; set; }

        /// <summary>Nombre de pila del paciente.</summary>
        [Required, MaxLength(50)]
        public required string FirstName { get; set; }

        /// <summary>Apellido del paciente.</summary>
        [Required, MaxLength(50)]
        public required string LastName { get; set; }

        /// <summary>Correo electrónico de contacto del paciente.</summary>
        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }

        /// <summary>Número de teléfono de contacto del paciente.</summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>Fecha de nacimiento para calcular edad y elegibilidad de tratamientos.</summary>
        public DateTime DateOfBirth { get; set; }

        // ─── Propiedades de navegación ───────────────────────────────────────

        /// <summary>Historial de citas del paciente (RF09).</summary>
        [JsonIgnore] public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        /// <summary>Expediente médico electrónico del paciente, creado al registrarse (RF11).</summary>
        [JsonIgnore] public MedicalRecord? MedicalRecord { get; set; }
    }
}
