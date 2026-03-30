using MedSync_API.Models;
using MedSync_API.Models.ViewModels;
using MedSync_API.Repositories;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de pacientes.
    /// Aplica las reglas de negocio del módulo de registro de pacientes (RF01, RF04, RF09, RF11).
    /// </summary>
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repo;
        private readonly IMedicalRecordRepository _expedienteRepo;

        public PatientService(IPatientRepository repo, IMedicalRecordRepository expedienteRepo)
        {
            _repo           = repo;
            _expedienteRepo = expedienteRepo;
        }

        /// <inheritdoc/>
        public async Task<PacienteViewModel?> ObtenerPorDocumentoAsync(string documentoId)
        {
            var paciente = await _repo.ObtenerPorDocumentoAsync(documentoId);
            return paciente is null ? null : MapearAViewModel(paciente);
        }

        /// <inheritdoc/>
        public async Task<PacienteViewModel?> ObtenerPorIdAsync(int id)
        {
            var paciente = await _repo.ObtenerPorIdAsync(id);
            return paciente is null ? null : MapearAViewModel(paciente);
        }

        /// <inheritdoc/>
        public async Task<PacienteViewModel> CrearAsync(PacienteCrearViewModel modelo)
        {
            // Regla de negocio: el documento de identidad identifica de forma única al paciente
            if (await _repo.ExisteDocumentoAsync(modelo.DocumentoId))
                throw new InvalidOperationException($"Ya existe un paciente registrado con el documento '{modelo.DocumentoId}'.");

            var paciente = new Patient
            {
                DocumentId  = modelo.DocumentoId,
                FirstName   = modelo.Nombre,
                LastName    = modelo.Apellido,
                Email       = modelo.Email,
                Phone       = modelo.Telefono,
                DateOfBirth = modelo.FechaNacimiento
            };

            await _repo.AgregarAsync(paciente);

            // RF11: Crear automáticamente el expediente médico electrónico del paciente
            var expediente = new MedicalRecord
            {
                PatientId = paciente.Id
            };
            await _expedienteRepo.AgregarAsync(expediente);

            return MapearAViewModel(paciente);
        }

        /// <inheritdoc/>
        public async Task ActualizarAsync(int id, PacienteActualizarViewModel modelo)
        {
            var paciente = await _repo.ObtenerPorIdAsync(id)
                ?? throw new KeyNotFoundException($"Paciente con id {id} no encontrado.");

            paciente.FirstName = modelo.Nombre;
            paciente.LastName  = modelo.Apellido;
            paciente.Email     = modelo.Email;
            paciente.Phone     = modelo.Telefono;

            await _repo.ActualizarAsync(paciente);
        }

        // ─── Mapeo de dominio a ViewModel ────────────────────────────────────

        private static PacienteViewModel MapearAViewModel(Patient p) => new()
        {
            Id              = p.Id,
            DocumentoId     = p.DocumentId,
            Nombre          = p.FirstName,
            Apellido        = p.LastName,
            Email           = p.Email,
            Telefono        = p.Phone,
            FechaNacimiento = p.DateOfBirth
        };
    }
}
