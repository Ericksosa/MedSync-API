using MedSync_API.Models;
using MedSync_API.Models.ViewModels;
using MedSync_API.Repositories;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de citas médicas.
    /// Aplica las reglas de negocio de la agenda: reserva, cancelación y estados.
    /// </summary>
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;

        /// <summary>
        /// Valores de estado válidos para una cita médica.
        /// La validación de transiciones se realiza en CambiarEstadoAsync.
        /// </summary>
        private static readonly string[] EstadosValidos =
            ["Pendiente", "Confirmada", "En Progreso", "Completada", "Cancelada", "No Presentó"];

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CitaViewModel>> ObtenerPorHospitalAsync(int hospitalId)
        {
            var citas = await _repo.ObtenerPorHospitalAsync(hospitalId);
            return citas.Select(MapearAViewModel);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CitaViewModel>> ObtenerPorPacienteAsync(int pacienteId)
        {
            var citas = await _repo.ObtenerPorPacienteAsync(pacienteId);
            return citas.Select(MapearAViewModel);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CitaViewModel>> ObtenerAgendaMedicoAsync(int medicoId, DateTime fecha)
        {
            var citas = await _repo.ObtenerPorMedicoYFechaAsync(medicoId, fecha);
            return citas.Select(MapearAViewModel);
        }

        /// <inheritdoc/>
        public async Task<CitaViewModel?> ObtenerPorIdAsync(int id)
        {
            var cita = await _repo.ObtenerPorIdAsync(id);
            return cita is null ? null : MapearAViewModel(cita);
        }

        /// <inheritdoc/>
        public async Task<CitaViewModel> CrearAsync(CitaCrearViewModel modelo)
        {
            // Regla de negocio: no se permite reservar en una franja ocupada
            if (await _repo.ExisteConflictoHorarioAsync(modelo.MedicoId, modelo.FechaHora))
                throw new InvalidOperationException("El médico ya tiene una cita programada en ese horario.");

            // Regla de negocio: no se pueden agendar citas en el pasado
            if (modelo.FechaHora < DateTime.UtcNow)
                throw new InvalidOperationException("No se pueden reservar citas en fechas pasadas.");

            var cita = new Appointment
            {
                DateTime   = modelo.FechaHora,
                Status     = "Pendiente",         // Estado inicial siempre es Pendiente
                Notes      = modelo.Notas,
                PatientId  = modelo.PacienteId,
                DoctorId   = modelo.MedicoId,
                HospitalId = modelo.HospitalId
            };

            await _repo.AgregarAsync(cita);
            return MapearAViewModel(cita);
        }

        /// <inheritdoc/>
        public async Task CambiarEstadoAsync(int id, string nuevoEstado)
        {
            // Valida que el estado solicitado sea uno de los definidos
            if (!EstadosValidos.Contains(nuevoEstado))
                throw new InvalidOperationException(
                    $"Estado '{nuevoEstado}' no válido. Los valores permitidos son: {string.Join(", ", EstadosValidos)}.");

            var cita = await _repo.ObtenerPorIdAsync(id)
                ?? throw new KeyNotFoundException($"Cita con id {id} no encontrada.");

            // Regla de negocio: una cita cancelada o completada no puede cambiar de estado
            if (cita.Status is "Cancelada" or "Completada")
                throw new InvalidOperationException(
                    $"No se puede cambiar el estado de una cita en estado '{cita.Status}'.");

            cita.Status = nuevoEstado;
            await _repo.ActualizarAsync(cita);
        }

        // ─── Mapeo de dominio a ViewModel ────────────────────────────────────

        private static CitaViewModel MapearAViewModel(Appointment a) => new()
        {
            Id             = a.Id,
            FechaHora      = a.DateTime,
            Estado         = a.Status,
            Notas          = a.Notes,
            PacienteId     = a.PatientId,
            NombrePaciente = a.Patient is not null ? $"{a.Patient.FirstName} {a.Patient.LastName}" : string.Empty,
            MedicoId       = a.DoctorId,
            NombreMedico   = a.Doctor is not null ? $"{a.Doctor.FirstName} {a.Doctor.LastName}" : string.Empty,
            HospitalId     = a.HospitalId,
            NombreHospital = a.Hospital?.Name ?? string.Empty
        };
    }
}
