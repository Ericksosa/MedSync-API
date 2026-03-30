using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de especialidades médicas.
    /// Gestiona el acceso a datos de las especialidades por hospital (RF21).
    /// </summary>
    public class SpecialtyRepository : GenericRepository<Specialty>, ISpecialtyRepository
    {
        public SpecialtyRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<IEnumerable<Specialty>> ObtenerPorHospitalAsync(int hospitalId)
        {
            // Lista las especialidades disponibles en el hospital para el flujo de reserva de citas (RF05)
            return await _dbSet
                .Where(s => s.HospitalId == hospitalId)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ExisteEnHospitalAsync(string nombre, int hospitalId)
        {
            // Evita duplicar especialidades con el mismo nombre dentro del mismo hospital
            return await _dbSet.AnyAsync(s =>
                s.Name.ToLower() == nombre.ToLower() &&
                s.HospitalId == hospitalId);
        }
    }
}
