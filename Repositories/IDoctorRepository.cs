using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Doctor.
    /// Extiende las operaciones CRUD genéricas con consultas propias del módulo (RF22).
    /// </summary>
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        /// <summary>Obtiene los médicos de un hospital específico con su especialidad incluida.</summary>
        Task<IEnumerable<Doctor>> ObtenerPorHospitalAsync(int hospitalId);

        /// <summary>Obtiene los médicos activos de una especialidad en un hospital.</summary>
        Task<IEnumerable<Doctor>> ObtenerActivosPorEspecialidadAsync(int especialidadId, int hospitalId);

        /// <summary>Verifica si ya existe un médico con el número de exequátur dado.</summary>
        Task<bool> ExisteExequaturAsync(string exequatur);

        /// <summary>Obtiene los hospitales asignados a un medico.</summary>
        Task<IEnumerable<Hospital>> ObtenerHospitalesAsignadosAsync(int doctorId);

        /// <summary>Actualiza los hospitales asignados a un medico.</summary>
        Task ActualizarHospitalesAsignadosAsync(int doctorId, IEnumerable<int> hospitalIds);

        /// <summary>Verifica si existe un hospital.</summary>
        Task<bool> ExisteHospitalAsync(int hospitalId);
    }
}
