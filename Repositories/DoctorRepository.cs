using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de médicos.
    /// Gestiona el acceso a datos del directorio de profesionales de la salud (RF22).
    /// </summary>
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<IEnumerable<Doctor>> ObtenerPorHospitalAsync(int hospitalId)
        {
            // Incluye la especialidad para enriquecer la respuesta sin llamadas adicionales
            return await _dbSet
                .Where(d => d.HospitalId == hospitalId || d.DoctorHospitals.Any(dh => dh.HospitalId == hospitalId))
                .Include(d => d.Specialty)
                .OrderBy(d => d.LastName)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Doctor>> ObtenerActivosPorEspecialidadAsync(int especialidadId, int hospitalId)
        {
            // Usada al reservar citas para mostrar solo médicos disponibles (RF05)
            return await _dbSet
                .Where(d => d.SpecialtyId == especialidadId
                         && (d.HospitalId == hospitalId || d.DoctorHospitals.Any(dh => dh.HospitalId == hospitalId))
                         && d.IsActive)
                .Include(d => d.Specialty)
                .OrderBy(d => d.LastName)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ExisteExequaturAsync(string exequatur)
        {
            // Valida unicidad del exequátur antes de registrar un nuevo médico
            return await _dbSet.AnyAsync(d => d.MedicalLicense == exequatur);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Hospital>> ObtenerHospitalesAsignadosAsync(int doctorId)
        {
            return await _context.DoctorHospitals
                .Where(dh => dh.DoctorId == doctorId)
                .Select(dh => dh.Hospital!)
                .OrderBy(h => h.Name)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task ActualizarHospitalesAsignadosAsync(int doctorId, IEnumerable<int> hospitalIds)
        {
            var ids = hospitalIds.Distinct().ToList();

            var actuales = await _context.DoctorHospitals
                .Where(dh => dh.DoctorId == doctorId)
                .ToListAsync();

            _context.DoctorHospitals.RemoveRange(actuales);

            foreach (var hospitalId in ids)
            {
                await _context.DoctorHospitals.AddAsync(new DoctorHospital
                {
                    DoctorId = doctorId,
                    HospitalId = hospitalId
                });
            }

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ExisteHospitalAsync(int hospitalId)
        {
            return await _context.Hospitals.AnyAsync(h => h.Id == hospitalId);
        }
    }
}
