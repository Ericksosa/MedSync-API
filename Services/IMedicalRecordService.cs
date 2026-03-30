using MedSync_API.Models.ViewModels;

namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de expedientes médicos.
    /// Maneja el acceso y actualización del historial clínico de los pacientes (RF11-RF15).
    /// </summary>
    public interface IMedicalRecordService
    {
        /// <summary>
        /// Obtiene el expediente médico de un paciente con diagnósticos y recetas.
        /// Cualquier médico del mismo hospital puede consultarlo (RF14).
        /// </summary>
        Task<ExpedienteViewModel?> ObtenerPorPacienteAsync(int pacienteId);

        /// <summary>
        /// Registra un nuevo diagnóstico en el expediente de un paciente (RF12).
        /// </summary>
        Task<DiagnosticoViewModel> RegistrarDiagnosticoAsync(DiagnosticoCrearViewModel modelo);

        /// <summary>
        /// Emite una receta médica y la vincula al expediente del paciente (RF13).
        /// </summary>
        Task<RecetaViewModel> EmitirRecetaAsync(RecetaCrearViewModel modelo);
    }
}
