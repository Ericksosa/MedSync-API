using MedSync_API.Models.ViewModels;

namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de hospitales.
    /// Define las operaciones de negocio del módulo de administración (RF20).
    /// </summary>
    public interface IHospitalService
    {
        /// <summary>Obtiene todos los hospitales registrados.</summary>
        Task<IEnumerable<HospitalViewModel>> ObtenerTodosAsync();

        /// <summary>Obtiene solo los hospitales activos (para selección en reservas).</summary>
        Task<IEnumerable<HospitalViewModel>> ObtenerActivosAsync();

        /// <summary>Obtiene un hospital por su identificador.</summary>
        Task<HospitalViewModel?> ObtenerPorIdAsync(int id);

        /// <summary>
        /// Registra un nuevo hospital en el sistema.
        /// Valida que el RNC no esté duplicado.
        /// </summary>
        Task<HospitalViewModel> CrearAsync(HospitalCrearViewModel modelo);

        /// <summary>Actualiza los datos de un hospital existente.</summary>
        Task ActualizarAsync(int id, HospitalActualizarViewModel modelo);
    }
}
