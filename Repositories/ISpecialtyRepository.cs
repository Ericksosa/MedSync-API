using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Specialty.
    /// Extiende las operaciones CRUD con consultas propias del módulo (RF21).
    /// </summary>
    public interface ISpecialtyRepository : IGenericRepository<Specialty>
    {
        /// <summary>Obtiene las especialidades disponibles en un hospital específico.</summary>
        Task<IEnumerable<Specialty>> ObtenerPorHospitalAsync(int hospitalId);

        /// <summary>Verifica si ya existe una especialidad con ese nombre en el hospital indicado.</summary>
        Task<bool> ExisteEnHospitalAsync(string nombre, int hospitalId);
    }
}
