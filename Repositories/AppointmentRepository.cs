using MedSync_API.Data;
using MedSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación del repositorio de citas médicas.
    /// Gestiona el acceso a datos de la agenda y el historial de consultas.
    /// </summary>
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<IEnumerable<Appointment>> ObtenerPorHospitalAsync(int hospitalId)
        {
            // Retorna citas con datos de paciente y médico incluidos para la vista de administración
            return await _dbSet
                .Where(a => a.HospitalId == hospitalId)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Appointment>> ObtenerPorMedicoYFechaAsync(int medicoId, DateTime fecha)
        {
            // Retorna la agenda diaria o semanal del médico para la vista
            return await _dbSet
                .Where(a => a.DoctorId == medicoId
                         && a.DateTime.Date == fecha.Date
                         && a.Status != "Cancelada")
                .Include(a => a.Patient)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Appointment>> ObtenerPorPacienteAsync(int pacienteId)
        {
            // Retorna el historial completo de citas del paciente
            return await _dbSet
                .Where(a => a.PatientId == pacienteId)
                .Include(a => a.Doctor)
                .Include(a => a.Hospital)
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ExisteConflictoHorarioAsync(int medicoId, DateTime fechaHora)
        {
            // Verifica solapamiento: no se permite crear otra cita al mismo médico en el mismo horario
            return await _dbSet.AnyAsync(a =>
                a.DoctorId == medicoId &&
                a.DateTime == fechaHora &&
                a.Status != "Cancelada" &&
                a.Status != "No Presentó");
        }
    }
}
