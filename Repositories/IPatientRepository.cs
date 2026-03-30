using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Patient.
    /// Extiende las operaciones CRUD genéricas con consultas propias del módulo (RF01, RF09).
    /// </summary>
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        /// <summary>Busca un paciente por su número de cédula o pasaporte.</summary>
        Task<Patient?> ObtenerPorDocumentoAsync(string documentoId);

        /// <summary>Verifica si ya existe un paciente registrado con ese documento de identidad.</summary>
        Task<bool> ExisteDocumentoAsync(string documentoId);
    }
}
