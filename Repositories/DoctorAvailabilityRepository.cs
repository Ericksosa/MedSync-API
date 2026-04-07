using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de disponibilidad de médicos.
    /// Gestiona las franjas horarias configuradas por médico (RF23).
    /// </summary>
    public class DoctorAvailabilityRepository : GenericRepository<DoctorAvailability>, IDoctorAvailabilityRepository
    {
        public DoctorAvailabilityRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<IEnumerable<DoctorAvailability>> ObtenerPorMedicoAsync(int doctorId)
        {
            // Retorna toda la agenda semanal del médico ordenada por día y hora
            return await _dbSet
                .Where(d => d.DoctorId == doctorId)
                .Include(d => d.Hospital)
                .OrderBy(d => d.DayOfWeek)
                .ThenBy(d => d.StartTime)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DoctorAvailability>> ObtenerPorMedicoYDiaAsync(int doctorId, int dayOfWeek)
        {
            // Filtra las franjas de un día específico para validar disponibilidad al reservar (RF05)
            return await _dbSet
                .Where(d => d.DoctorId == doctorId && d.DayOfWeek == dayOfWeek)
                .Include(d => d.Hospital)
                .OrderBy(d => d.StartTime)
                .ToListAsync();
        }
    }
}
