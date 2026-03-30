using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad MedicalRecord (expediente médico).
    /// Maneja el acceso al expediente electrónico del paciente (RF11, RF14).
    /// </summary>
    public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord>
    {
        /// <summary>
        /// Obtiene el expediente médico de un paciente junto con sus diagnósticos y recetas.
        /// Cualquier médico del mismo hospital puede consultarlo (RF14).
        /// </summary>
        Task<MedicalRecord?> ObtenerPorPacienteAsync(int pacienteId);

        /// <summary>Verifica si un paciente ya tiene un expediente médico registrado.</summary>
        Task<bool> TieneExpedienteAsync(int pacienteId);
    }
}
