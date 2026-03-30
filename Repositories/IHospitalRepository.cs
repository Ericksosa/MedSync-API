using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Hospital.
    /// Extiende las operaciones CRUD genéricas con consultas propias del módulo (RF20).
    /// </summary>
    public interface IHospitalRepository : IGenericRepository<Hospital>
    {
        /// <summary>Obtiene solo los hospitales marcados como activos en el sistema.</summary>
        Task<IEnumerable<Hospital>> ObtenerActivosAsync();

        /// <summary>Verifica si ya existe un hospital con el RNC indicado.</summary>
        Task<bool> ExisteRncAsync(string rnc);
    }
}
