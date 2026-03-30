using MedSync_API.Models.ViewModels;

namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de médicos.
    /// Define las operaciones de negocio del módulo de administración de doctores (RF22, RF23).
    /// </summary>
    public interface IDoctorService
    {
        /// <summary>Obtiene los médicos de un hospital específico.</summary>
        Task<IEnumerable<MedicoViewModel>> ObtenerPorHospitalAsync(int hospitalId);

        /// <summary>Obtiene los médicos activos de una especialidad en un hospital.</summary>
        Task<IEnumerable<MedicoViewModel>> ObtenerActivosPorEspecialidadAsync(int especialidadId, int hospitalId);

        /// <summary>Obtiene un médico por su identificador.</summary>
        Task<MedicoViewModel?> ObtenerPorIdAsync(int id);

        /// <summary>
        /// Registra un nuevo médico en el sistema.
        /// Valida que el exequátur no esté duplicado.
        /// </summary>
        Task<MedicoViewModel> CrearAsync(MedicoCrearViewModel modelo);

        /// <summary>Activa o desactiva el perfil de un médico (RF22).</summary>
        Task CambiarEstadoAsync(int id, bool estaActivo);

        /// <summary>Obtiene la disponibilidad horaria de un médico (RF23).</summary>
        Task<IEnumerable<DisponibilidadViewModel>> ObtenerDisponibilidadAsync(int medicoId);

        /// <summary>Configura una franja horaria de disponibilidad para el médico (RF23).</summary>
        Task<DisponibilidadViewModel> AgregarDisponibilidadAsync(DisponibilidadCrearViewModel modelo);

        /// <summary>Elimina una franja de disponibilidad del médico (RF23).</summary>
        Task EliminarDisponibilidadAsync(int disponibilidadId);
    }
}
