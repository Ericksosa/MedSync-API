using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de diagnósticos médicos.
    /// Gestiona el acceso a los diagnósticos registrados en los expedientes (RF12).
    /// </summary>
    public class DiagnosisRepository : GenericRepository<Diagnosis>, IDiagnosisRepository
    {
        public DiagnosisRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<IEnumerable<Diagnosis>> ObtenerPorExpedienteAsync(int medicalRecordId)
        {
            // Lista todos los diagnósticos del expediente, con el médico que los registró
            return await _dbSet
                .Where(d => d.MedicalRecordId == medicalRecordId)
                .Include(d => d.Doctor)
                .OrderByDescending(d => d.RecordedAt)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Diagnosis>> ObtenerPorMedicoAsync(int doctorId)
        {
            // Permite al médico consultar su propio historial de diagnósticos emitidos
            return await _dbSet
                .Where(d => d.DoctorId == doctorId)
                .Include(d => d.MedicalRecord)
                .OrderByDescending(d => d.RecordedAt)
                .ToListAsync();
        }
    }
}
