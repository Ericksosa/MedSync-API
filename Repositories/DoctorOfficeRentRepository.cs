using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de cobros de consultorio.
    /// </summary>
    public class DoctorOfficeRentRepository : GenericRepository<DoctorOfficeRent>, IDoctorOfficeRentRepository
    {
        public DoctorOfficeRentRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DoctorOfficeRent>> ObtenerPorHospitalAsync(int hospitalId)
        {
            return await _dbSet
                .Where(c => c.HospitalId == hospitalId)
                .Include(c => c.Doctor)
                .Include(c => c.Hospital)
                .OrderByDescending(c => c.DueDate)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DoctorOfficeRent>> ObtenerPorDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Where(c => c.DoctorId == doctorId)
                .Include(c => c.Doctor)
                .Include(c => c.Hospital)
                .OrderByDescending(c => c.DueDate)
                .ToListAsync();
        }
    }
}