using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad Appointment.
    /// Extiende las operaciones CRUD con consultas de agenda y disponibilidad (RF05, RF08, RF09).
    /// </summary>
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        /// <summary>Obtiene las citas de un hospital con datos del paciente y médico incluidos.</summary>
        Task<IEnumerable<Appointment>> ObtenerPorHospitalAsync(int hospitalId);

        /// <summary>Obtiene las citas activas (no canceladas) de un médico en una fecha dada.</summary>
        Task<IEnumerable<Appointment>> ObtenerPorMedicoYFechaAsync(int medicoId, DateTime fecha);

        /// <summary>Obtiene el historial completo de citas de un paciente.</summary>
        Task<IEnumerable<Appointment>> ObtenerPorPacienteAsync(int pacienteId);

        /// <summary>
        /// Verifica si un médico ya tiene una cita en el horario solicitado.
        /// Utilizado para evitar solapamiento de citas (RF05).
        /// </summary>
        Task<bool> ExisteConflictoHorarioAsync(int medicoId, DateTime fechaHora);
    }
}
