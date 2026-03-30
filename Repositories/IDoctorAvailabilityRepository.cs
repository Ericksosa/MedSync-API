using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad DoctorAvailability (disponibilidad del médico).
    /// Gestiona el acceso a los horarios disponibles de cada médico (RF23).
    /// </summary>
    public interface IDoctorAvailabilityRepository : IGenericRepository<DoctorAvailability>
    {
        /// <summary>Obtiene todas las franjas horarias configuradas para un médico.</summary>
        Task<IEnumerable<DoctorAvailability>> ObtenerPorMedicoAsync(int medicoId);

        /// <summary>Obtiene la disponibilidad de un médico para un día específico de la semana.</summary>
        Task<IEnumerable<DoctorAvailability>> ObtenerPorMedicoYDiaAsync(int medicoId, int diaSemana);
    }
}
