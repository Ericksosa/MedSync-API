using MedSync_API.Models;
using MedSync_API.Models.ViewModels;
using MedSync_API.Repositories;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de expedientes médicos.
    /// Gestiona el historial clínico: diagnósticos y recetas de los pacientes (RF11-RF15).
    /// </summary>
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _expedienteRepo;
        private readonly IDiagnosisRepository     _diagnosticoRepo;
        private readonly IPrescriptionRepository  _recetaRepo;

        public MedicalRecordService(
            IMedicalRecordRepository expedienteRepo,
            IDiagnosisRepository diagnosticoRepo,
            IPrescriptionRepository recetaRepo)
        {
            _expedienteRepo  = expedienteRepo;
            _diagnosticoRepo = diagnosticoRepo;
            _recetaRepo      = recetaRepo;
        }

        /// <inheritdoc/>
        public async Task<ExpedienteViewModel?> ObtenerPorPacienteAsync(int pacienteId)
        {
            var expediente = await _expedienteRepo.ObtenerPorPacienteAsync(pacienteId);
            if (expediente is null) return null;

            return new ExpedienteViewModel
            {
                Id             = expediente.Id,
                PacienteId     = expediente.PatientId,
                NombrePaciente = expediente.Patient is not null
                    ? $"{expediente.Patient.FirstName} {expediente.Patient.LastName}"
                    : string.Empty,
                CreadoEn     = expediente.CreatedAt,
                Diagnosticos = expediente.Diagnoses.Select(MapearDiagnosticoAViewModel),
                Recetas      = expediente.Prescriptions.Select(MapearRecetaAViewModel)
            };
        }

        /// <inheritdoc/>
        public async Task<DiagnosticoViewModel> RegistrarDiagnosticoAsync(DiagnosticoCrearViewModel modelo)
        {
            // Verifica que el expediente exista antes de agregar el diagnóstico (RF12)
            var expediente = await _expedienteRepo.ObtenerPorIdAsync(modelo.ExpedienteId)
                ?? throw new KeyNotFoundException($"Expediente médico con id {modelo.ExpedienteId} no encontrado.");

            var diagnostico = new Diagnosis
            {
                MedicalRecordId = modelo.ExpedienteId,
                Description     = modelo.Descripcion,
                DoctorId        = modelo.MedicoId,
                AppointmentId   = modelo.CitaId
            };

            await _diagnosticoRepo.AgregarAsync(diagnostico);
            return MapearDiagnosticoAViewModel(diagnostico);
        }

        /// <inheritdoc/>
        public async Task<RecetaViewModel> EmitirRecetaAsync(RecetaCrearViewModel modelo)
        {
            // Verifica que el expediente exista antes de emitir la receta (RF13)
            var expediente = await _expedienteRepo.ObtenerPorIdAsync(modelo.ExpedienteId)
                ?? throw new KeyNotFoundException($"Expediente médico con id {modelo.ExpedienteId} no encontrado.");

            var receta = new Prescription
            {
                MedicalRecordId = modelo.ExpedienteId,
                Medication      = modelo.Medicamento,
                Dosage          = modelo.Dosis,
                Instructions    = modelo.Instrucciones,
                DoctorId        = modelo.MedicoId,
                AppointmentId   = modelo.CitaId
            };

            await _recetaRepo.AgregarAsync(receta);
            return MapearRecetaAViewModel(receta);
        }

        // ─── Mapeo de dominio a ViewModel ────────────────────────────────────

        private static DiagnosticoViewModel MapearDiagnosticoAViewModel(Diagnosis d) => new()
        {
            Id            = d.Id,
            ExpedienteId  = d.MedicalRecordId,
            Descripcion   = d.Description,
            MedicoId      = d.DoctorId,
            NombreMedico  = d.Doctor is not null ? $"{d.Doctor.FirstName} {d.Doctor.LastName}" : string.Empty,
            CitaId        = d.AppointmentId,
            FechaRegistro = d.RecordedAt
        };

        private static RecetaViewModel MapearRecetaAViewModel(Prescription r) => new()
        {
            Id            = r.Id,
            ExpedienteId  = r.MedicalRecordId,
            Medicamento   = r.Medication,
            Dosis         = r.Dosage,
            Instrucciones = r.Instructions,
            MedicoId      = r.DoctorId,
            NombreMedico  = r.Doctor is not null ? $"{r.Doctor.FirstName} {r.Doctor.LastName}" : string.Empty,
            CitaId        = r.AppointmentId,
            FechaEmision  = r.IssuedAt,
            EstaActiva    = r.IsActive
        };
    }
}
