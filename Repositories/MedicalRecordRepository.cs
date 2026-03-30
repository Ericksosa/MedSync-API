using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de expedientes médicos.
    /// Gestiona el acceso al historial clínico completo de los pacientes (RF11, RF14).
    /// </summary>
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<MedicalRecord?> ObtenerPorPacienteAsync(int patientId)
        {
            // Carga diagnósticos y recetas en una sola consulta para la vista del expediente (RF14)
            return await _dbSet
                .Where(e => e.PatientId == patientId)
                .Include(e => e.Diagnoses)
                    .ThenInclude(d => d.Doctor)
                .Include(e => e.Prescriptions)
                    .ThenInclude(r => r.Doctor)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> TieneExpedienteAsync(int patientId)
        {
            // Verifica existencia del expediente antes de crear uno duplicado (RF11)
            return await _dbSet.AnyAsync(e => e.PatientId == patientId);
        }
    }
}
