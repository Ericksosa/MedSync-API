using System.ComponentModel.DataAnnotations;

namespace MedSync_API.Models.ViewModels
{
    /// <summary>
    /// ViewModel de respuesta con los datos del pago de una cita.
    /// </summary>
    public class PagoViewModel
    {
        public int Id { get; set; }
        public int CitaId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para registrar el pago de una cita (RF16).
    /// Valores válidos para MetodoPago: "Card", "Cash", "Insurance".
    /// Valores válidos para Estado: "Completed", "Pending", "Refunded".
    /// </summary>
    public class PagoCrearViewModel
    {
        [Required(ErrorMessage = "El identificador de la cita es obligatorio.")]
        public int CitaId { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Monto { get; set; }

        /// <summary>Método de pago: "Card", "Cash" o "Insurance".</summary>
        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        [MaxLength(50)]
        public required string MetodoPago { get; set; }

        /// <summary>Estado inicial del pago: "Pending" o "Completed".</summary>
        [Required(ErrorMessage = "El estado del pago es obligatorio.")]
        [MaxLength(20)]
        public required string Estado { get; set; }
    }
}
