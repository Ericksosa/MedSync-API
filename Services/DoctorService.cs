using MedSync_API.Models;
using MedSync_API.Models.ViewModels;
using MedSync_API.Repositories;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de médicos.
    /// Aplica las reglas de negocio del directorio de profesionales (RF22, RF23).
    /// </summary>
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repo;
        private readonly IDoctorAvailabilityRepository _disponibilidadRepo;

        public DoctorService(IDoctorRepository repo, IDoctorAvailabilityRepository disponibilidadRepo)
        {
            _repo              = repo;
            _disponibilidadRepo = disponibilidadRepo;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MedicoViewModel>> ObtenerPorHospitalAsync(int hospitalId)
        {
            var medicos = await _repo.ObtenerPorHospitalAsync(hospitalId);
            return medicos.Select(MapearAViewModel);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MedicoViewModel>> ObtenerActivosPorEspecialidadAsync(int especialidadId, int hospitalId)
        {
            var medicos = await _repo.ObtenerActivosPorEspecialidadAsync(especialidadId, hospitalId);
            return medicos.Select(MapearAViewModel);
        }

        /// <inheritdoc/>
        public async Task<MedicoViewModel?> ObtenerPorIdAsync(int id)
        {
            var medico = await _repo.ObtenerPorIdAsync(id);
            return medico is null ? null : MapearAViewModel(medico);
        }

        /// <inheritdoc/>
        public async Task<MedicoViewModel> CrearAsync(MedicoCrearViewModel modelo)
        {
            // Regla de negocio: el exequátur identifica de forma única a un médico
            if (await _repo.ExisteExequaturAsync(modelo.Exequatur))
                throw new InvalidOperationException($"Ya existe un médico con el exequátur '{modelo.Exequatur}'.");

            var medico = new Doctor
            {
                FirstName      = modelo.Nombre,
                LastName       = modelo.Apellido,
                MedicalLicense = modelo.Exequatur,
                SpecialtyId    = modelo.EspecialidadId,
                HospitalId     = modelo.HospitalId
            };

            await _repo.AgregarAsync(medico);
            return MapearAViewModel(medico);
        }

        /// <inheritdoc/>
        public async Task CambiarEstadoAsync(int id, bool estaActivo)
        {
            var medico = await _repo.ObtenerPorIdAsync(id)
                ?? throw new KeyNotFoundException($"Médico con id {id} no encontrado.");

            medico.IsActive = estaActivo;
            await _repo.ActualizarAsync(medico);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DisponibilidadViewModel>> ObtenerDisponibilidadAsync(int medicoId)
        {
            var disponibilidades = await _disponibilidadRepo.ObtenerPorMedicoAsync(medicoId);
            return disponibilidades.Select(MapearDisponibilidadAViewModel);
        }

        /// <inheritdoc/>
        public async Task<DisponibilidadViewModel> AgregarDisponibilidadAsync(DisponibilidadCrearViewModel modelo)
        {
            // Regla de negocio: la hora de fin debe ser posterior a la hora de inicio
            if (modelo.HoraFin <= modelo.HoraInicio)
                throw new InvalidOperationException("La hora de fin debe ser posterior a la hora de inicio.");

            var disponibilidad = new DoctorAvailability
            {
                DoctorId  = modelo.MedicoId,
                DayOfWeek = modelo.DiaSemana,
                StartTime = modelo.HoraInicio,
                EndTime   = modelo.HoraFin
            };

            await _disponibilidadRepo.AgregarAsync(disponibilidad);
            return MapearDisponibilidadAViewModel(disponibilidad);
        }

        /// <inheritdoc/>
        public async Task EliminarDisponibilidadAsync(int disponibilidadId)
        {
            var disponibilidad = await _disponibilidadRepo.ObtenerPorIdAsync(disponibilidadId)
                ?? throw new KeyNotFoundException($"Disponibilidad con id {disponibilidadId} no encontrada.");

            await _disponibilidadRepo.EliminarAsync(disponibilidad);
        }

        // ─── Mapeo de dominio a ViewModel ────────────────────────────────────

        private static MedicoViewModel MapearAViewModel(Doctor d) => new()
        {
            Id                = d.Id,
            Nombre            = d.FirstName,
            Apellido          = d.LastName,
            Exequatur         = d.MedicalLicense,
            EspecialidadId    = d.SpecialtyId,
            NombreEspecialidad = d.Specialty?.Name ?? string.Empty,
            HospitalId        = d.HospitalId,
            NombreHospital    = d.Hospital?.Name ?? string.Empty,
            EstaActivo        = d.IsActive
        };

        private static DisponibilidadViewModel MapearDisponibilidadAViewModel(DoctorAvailability d) => new()
        {
            Id           = d.Id,
            MedicoId     = d.DoctorId,
            NombreMedico = d.Doctor is not null ? $"{d.Doctor.FirstName} {d.Doctor.LastName}" : string.Empty,
            DiaSemana    = d.DayOfWeek,
            HoraInicio   = d.StartTime,
            HoraFin      = d.EndTime
        };
    }
}
