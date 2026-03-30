using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Prescription (receta médica).
    /// Gestiona el acceso a las recetas emitidas en los expedientes (RF13, RF15).
    /// </summary>
    public interface IPrescriptionRepository : IGenericRepository<Prescription>
    {
        /// <summary>Obtiene todas las recetas de un expediente médico específico.</summary>
        Task<IEnumerable<Prescription>> ObtenerPorExpedienteAsync(int expedienteId);

        /// <summary>Obtiene las recetas activas vigentes del expediente del paciente.</summary>
        Task<IEnumerable<Prescription>> ObtenerActivasPorExpedienteAsync(int expedienteId);
    }
}
