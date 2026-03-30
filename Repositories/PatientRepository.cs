using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de pacientes.
    /// Gestiona el acceso a datos de los pacientes registrados en el sistema (RF01, RF09).
    /// </summary>
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<Patient?> ObtenerPorDocumentoAsync(string documentoId)
        {
            // Permite buscar al paciente por su cédula o pasaporte en el proceso de admisión
            return await _dbSet
                .FirstOrDefaultAsync(p => p.DocumentId == documentoId);
        }

        /// <inheritdoc/>
        public async Task<bool> ExisteDocumentoAsync(string documentoId)
        {
            // Valida que no se dupliquen pacientes con el mismo documento de identidad
            return await _dbSet.AnyAsync(p => p.DocumentId == documentoId);
        }
    }
}
