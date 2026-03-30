using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de recetas médicas.
    /// Gestiona el acceso a las recetas emitidas en los expedientes (RF13, RF15).
    /// </summary>
    public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<IEnumerable<Prescription>> ObtenerPorExpedienteAsync(int medicalRecordId)
        {
            // Lista todas las recetas del expediente con el médico que las emitió (RF15)
            return await _dbSet
                .Where(r => r.MedicalRecordId == medicalRecordId)
                .Include(r => r.Doctor)
                .OrderByDescending(r => r.IssuedAt)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Prescription>> ObtenerActivasPorExpedienteAsync(int medicalRecordId)
        {
            // Retorna solo las recetas vigentes, útil para la vista de medicación activa del paciente
            return await _dbSet
                .Where(r => r.MedicalRecordId == medicalRecordId && r.IsActive)
                .Include(r => r.Doctor)
                .OrderByDescending(r => r.IssuedAt)
                .ToListAsync();
        }
    }
}
