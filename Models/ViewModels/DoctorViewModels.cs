using System.ComponentModel.DataAnnotations;

namespace MedSync_API.Models.ViewModels
{
    /// <summary>
    /// ViewModel de respuesta con datos del médico enriquecidos.
    /// Incluye los nombres de especialidad y hospital para evitar llamadas adicionales.
    /// </summary>
    public class MedicoViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public string Exequatur { get; set; } = string.Empty;
        public int EspecialidadId { get; set; }
        public string NombreEspecialidad { get; set; } = string.Empty;
        public int HospitalId { get; set; }
        public string NombreHospital { get; set; } = string.Empty;
        public bool EstaActivo { get; set; }
    }

    /// <summary>
    /// ViewModel de entrada para registrar un nuevo médico (RF22).
    /// </summary>
    public class MedicoCrearViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50)]
        public required string Apellido { get; set; }

        /// <summary>Número de exequátur emitido por el Ministerio de Salud.</summary>
        [Required(ErrorMessage = "El número de exequátur es obligatorio.")]
        [MaxLength(50)]
        public required string Exequatur { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        public int EspecialidadId { get; set; }

        [Required(ErrorMessage = "El hospital es obligatorio.")]
        public int HospitalId { get; set; }
    }

    /// <summary>
    /// ViewModel para cambiar el estado activo/inactivo de un médico (RF22).
    /// </summary>
    public class MedicoEstadoViewModel
    {
        /// <summary>Nuevo estado del médico: true = activo, false = inactivo.</summary>
        [Required]
        public bool EstaActivo { get; set; }
    }

    /// <summary>
    /// ViewModel para representar un hospital asignado a un medico.
    /// </summary>
    public class HospitalAsignadoViewModel
    {
        public int HospitalId { get; set; }
        public string NombreHospital { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para actualizar la lista de hospitales asignados al medico.
    /// </summary>
    public class HospitalesAsignadosActualizarViewModel
    {
        [Required(ErrorMessage = "Debe enviar al menos un hospital.")]
        public List<int> HospitalesIds { get; set; } = new();
    }
}
