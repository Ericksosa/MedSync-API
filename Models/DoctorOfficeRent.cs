using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedSync_API.Models
{
    /// <summary>
    /// Cobro periódico al médico por el uso de su consultorio en el hospital (RF17, RF18).
    /// El administrador gestiona y registra el estado de cada cobro.
    /// </summary>
    [Table("DoctorOfficeRents")]
    public class DoctorOfficeRent
    {
        /// <summary>Identificador único del cobro de consultorio.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Médico al que se factura el uso del consultorio.</summary>
        [Required]
        public int DoctorId { get; set; }

        /// <summary>Hospital donde se ubica el consultorio.</summary>
        [Required]
        public int HospitalId { get; set; }

        /// <summary>Monto facturado al médico por el período correspondiente.</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Periodicidad del cobro.
        /// Valores válidos: "Mensual", "Quincenal", "Semanal".
        /// </summary>
        [Required, MaxLength(20)]
        public required string Period { get; set; }

        /// <summary>
        /// Estado actual del cobro.
        /// Valores válidos: "Pendiente", "Pagado".
        /// </summary>
        [Required, MaxLength(20)]
        public required string Status { get; set; }

        /// <summary>Fecha límite de pago de este cobro.</summary>
        [Required]
        public DateTime DueDate { get; set; }

        /// <summary>Fecha en que el médico realizó el pago (nulo si aún no ha pagado).</summary>
        public DateTime? PaymentDate { get; set; }

        // ─── Propiedades de navegación ───────────────────────────────────────

        /// <summary>Médico al que pertenece este cobro.</summary>
        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        /// <summary>Hospital donde se encuentra el consultorio.</summary>
        [ForeignKey("HospitalId")]
        public Hospital? Hospital { get; set; }
    }
}
