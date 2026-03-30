using MedSync_API.Models.ViewModels;

namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de pacientes.
    /// Define las operaciones de negocio del módulo de pacientes (RF01, RF04, RF09).
    /// </summary>
    public interface IPatientService
    {
        /// <summary>
        /// Busca un paciente por su documento de identidad (cédula o pasaporte).
        /// Retorna null si no existe.
        /// </summary>
        Task<PacienteViewModel?> ObtenerPorDocumentoAsync(string documentoId);

        /// <summary>Obtiene un paciente por su identificador interno.</summary>
        Task<PacienteViewModel?> ObtenerPorIdAsync(int id);

        /// <summary>
        /// Registra un nuevo paciente y crea automáticamente su expediente médico (RF01, RF11).
        /// Valida que no exista otro paciente con el mismo documento de identidad.
        /// </summary>
        Task<PacienteViewModel> CrearAsync(PacienteCrearViewModel modelo);

        /// <summary>Actualiza los datos personales de un paciente (RF04).</summary>
        Task ActualizarAsync(int id, PacienteActualizarViewModel modelo);
    }
}
