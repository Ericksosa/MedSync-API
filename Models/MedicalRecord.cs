using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedSync_API.Models
{
    /// <summary>
    /// Expediente médico electrónico del paciente (RF11).
    /// Se crea automáticamente al registrar un nuevo paciente.
    /// Contiene el historial de diagnósticos y recetas emitidas.
    /// </summary>
    [Table("MedicalRecords")]
    public class MedicalRecord
    {
        /// <summary>Identificador único del expediente médico.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Referencia al paciente propietario del expediente.</summary>
        [Required]
        public int PatientId { get; set; }

        /// <summary>Fecha y hora en que fue creado el expediente (UTC).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Ritmo cardiaco registrado (latidos por minuto).</summary>
        public int? HeartRate { get; set; }

        /// <summary>Temperatura corporal registrada en grados Celsius.</summary>
        public decimal? Temperature { get; set; }

        /// <summary>Presion arterial registrada. Ejemplo: 120/80.</summary>
        [MaxLength(20)]
        public string? BloodPressure { get; set; }

        /// <summary>Fecha y hora de la ultima actualizacion de signos vitales (UTC).</summary>
        public DateTime? LastVitalsUpdatedAt { get; set; }

        // ─── Propiedades de navegación ───────────────────────────────────────

        /// <summary>Paciente al que pertenece este expediente.</summary>
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        /// <summary>Lista de diagnósticos registrados en este expediente (RF12).</summary>
        [JsonIgnore]
        public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();

        /// <summary>Lista de recetas emitidas vinculadas a este expediente (RF13).</summary>
        [JsonIgnore]
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
