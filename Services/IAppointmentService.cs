using MedSync_API.Models.ViewModels;

namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de citas médicas.
    /// Encapsula las reglas de negocio del módulo de agenda (RF05, RF06, RF07, RF08, RF09, RF10).
    /// </summary>
    public interface IAppointmentService
    {
        /// <summary>Obtiene todas las citas de un hospital con datos enriquecidos.</summary>
        Task<IEnumerable<CitaViewModel>> ObtenerPorHospitalAsync(int hospitalId);

        /// <summary>Obtiene las citas de un paciente (historial, RF09).</summary>
        Task<IEnumerable<CitaViewModel>> ObtenerPorPacienteAsync(int pacienteId);

        /// <summary>Obtiene la agenda del médico en una fecha dada (RF08).</summary>
        Task<IEnumerable<CitaViewModel>> ObtenerAgendaMedicoAsync(int medicoId, DateTime fecha);

        /// <summary>Obtiene una cita por su identificador.</summary>
        Task<CitaViewModel?> ObtenerPorIdAsync(int id);

        /// <summary>
        /// Reserva una nueva cita médica (RF05).
        /// Valida disponibilidad del médico y que no exista conflicto de horario.
        /// El estado inicial siempre es "Pendiente".
        /// </summary>
        Task<CitaViewModel> CrearAsync(CitaCrearViewModel modelo);

        /// <summary>
        /// Cambia el estado de una cita (RF07, RF10).
        /// Valida que la transición de estado sea permitida por las reglas de negocio.
        /// </summary>
        Task CambiarEstadoAsync(int id, string nuevoEstado);
    }
}
