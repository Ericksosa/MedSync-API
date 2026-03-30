using MedSync_API.Models.ViewModels;

namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de especialidades médicas.
    /// Define las operaciones de negocio del módulo de administración (RF21).
    /// </summary>
    public interface ISpecialtyService
    {
        /// <summary>Obtiene las especialidades de un hospital específico.</summary>
        Task<IEnumerable<EspecialidadViewModel>> ObtenerPorHospitalAsync(int hospitalId);

        /// <summary>
        /// Crea una nueva especialidad médica en un hospital (RF21).
        /// Valida que no exista otra especialidad con el mismo nombre en ese hospital.
        /// </summary>
        Task<EspecialidadViewModel> CrearAsync(EspecialidadCrearViewModel modelo);
    }
}
