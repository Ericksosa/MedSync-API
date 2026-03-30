using System.ComponentModel.DataAnnotations;

namespace MedSync_API.Models.ViewModels
{
    /// <summary>
    /// ViewModel de respuesta para un hospital.
    /// Se devuelve al cliente en lugar del modelo de dominio crudo.
    /// </summary>
    public class HospitalViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Rnc { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public bool EstaActivo { get; set; }
        public DateTime CreadoEn { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para crear un nuevo hospital (RF20).
    /// </summary>
    public class HospitalCrearViewModel
    {
        /// <summary>Nombre comercial del hospital.</summary>
        [Required(ErrorMessage = "El nombre del hospital es obligatorio.")]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        /// <summary>Registro Nacional del Contribuyente (RNC).</summary>
        [Required(ErrorMessage = "El RNC es obligatorio.")]
        [MaxLength(20)]
        public required string Rnc { get; set; }

        [MaxLength(255)]
        public string? Direccion { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para actualizar un hospital existente (RF20).
    /// </summary>
    public class HospitalActualizarViewModel
    {
        [Required(ErrorMessage = "El nombre del hospital es obligatorio.")]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El RNC es obligatorio.")]
        [MaxLength(20)]
        public required string Rnc { get; set; }

        [MaxLength(255)]
        public string? Direccion { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        /// <summary>Indica si el hospital debe estar activo o inactivo.</summary>
        public bool EstaActivo { get; set; }
    }
}
