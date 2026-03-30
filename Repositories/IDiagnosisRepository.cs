using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Diagnosis (diagnóstico médico).
    /// Gestiona el acceso a los diagnósticos registrados en los expedientes (RF12).
    /// </summary>
    public interface IDiagnosisRepository : IGenericRepository<Diagnosis>
    {
        /// <summary>Obtiene todos los diagnósticos de un expediente médico específico.</summary>
        Task<IEnumerable<Diagnosis>> ObtenerPorExpedienteAsync(int expedienteId);

        /// <summary>Obtiene los diagnósticos registrados por un médico en particular.</summary>
        Task<IEnumerable<Diagnosis>> ObtenerPorMedicoAsync(int medicoId);
    }
}
