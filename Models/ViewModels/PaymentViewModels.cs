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
        public DateTime FechaCita { get; set; }
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreMedico { get; set; } = string.Empty;
        public string NombreHospital { get; set; } = string.Empty;
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

        /// <summary>
        /// Estado inicial del pago.
        /// Los pagos se crean siempre en "Pending" y luego se confirman como "Completed".
        /// </summary>
        [MaxLength(20)]
        public string Estado { get; set; } = "Pending";
    }

    /// <summary>
    /// ViewModel para actualizar el estado de un pago.
    /// </summary>
    public class PagoEstadoViewModel
    {
        [Required(ErrorMessage = "El estado del pago es obligatorio.")]
        [MaxLength(20)]
        public required string Estado { get; set; }
    }

    /// <summary>
    /// ViewModel de respuesta para cobros de consultorio a médicos (RF17).
    /// </summary>
    public class CobroViewModel
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public int HospitalId { get; set; }
        public string NombreHospital { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaPago { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para crear un cobro de consultorio (RF17).
    /// El período debe ser uno de los 12 meses del año en español.
    /// </summary>
    public class CobroCrearViewModel
    {
        [Required(ErrorMessage = "El médico es obligatorio.")]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "El hospital es obligatorio.")]
        public int HospitalId { get; set; }

        [Required(ErrorMessage = "El mes del cobro es obligatorio.")]
        [MaxLength(20)]
        public required string Periodo { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente";
    }

    /// <summary>
    /// ViewModel de entrada para actualizar estado de cobro.
    /// </summary>
    public class CobroEstadoViewModel
    {
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(20)]
        public required string Estado { get; set; }
    }

    /// <summary>
    /// Resumen financiero por médico para estado de cuenta (RF18).
    /// </summary>
    public class BalanceMedicoViewModel
    {
        public string NombreMedico { get; set; } = string.Empty;
        public decimal TotalGenerado { get; set; }
        public decimal TotalCobrado { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal Pendiente => TotalCobrado - TotalPagado;
    }
}
