using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de hospitales.
    /// Gestiona el acceso a datos de los centros médicos registrados en el sistema (RF20).
    /// </summary>
    public class HospitalRepository : GenericRepository<Hospital>, IHospitalRepository
    {
        public HospitalRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<IEnumerable<Hospital>> ObtenerActivosAsync()
        {
            // Filtra solo los hospitales operativos para las vistas de selección del paciente
            return await _dbSet
                .Where(h => h.IsActive)
                .OrderBy(h => h.Name)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ExisteRncAsync(string rnc)
        {
            // Valida unicidad del RNC antes de registrar un nuevo hospital
            return await _dbSet.AnyAsync(h => h.Rnc == rnc);
        }
    }
}
