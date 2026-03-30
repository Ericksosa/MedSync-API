using System.ComponentModel.DataAnnotations;

namespace MedSync_API.Models.ViewModels
{
    /// <summary>
    /// ViewModel de respuesta con los datos de una especialidad médica.
    /// </summary>
    public class EspecialidadViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int HospitalId { get; set; }
        public string NombreHospital { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel de entrada para crear una nueva especialidad médica (RF21).
    /// </summary>
    public class EspecialidadCrearViewModel
    {
        [Required(ErrorMessage = "El nombre de la especialidad es obligatorio.")]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El hospital es obligatorio.")]
        public int HospitalId { get; set; }
    }
}
