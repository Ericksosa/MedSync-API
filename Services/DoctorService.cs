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
            await _repo.ActualizarHospitalesAsignadosAsync(medico.Id, new[] { modelo.HospitalId });
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
        public async Task<IEnumerable<DisponibilidadViewModel>> ObtenerDisponibilidadAsync(int medicoId, int? hospitalId = null)
        {
            var disponibilidades = await _disponibilidadRepo.ObtenerPorMedicoAsync(medicoId);

            if (hospitalId.HasValue)
                disponibilidades = disponibilidades.Where(d => d.HospitalId == hospitalId.Value);

            return disponibilidades.Select(MapearDisponibilidadAViewModel);
        }

        /// <inheritdoc/>
        public async Task<DisponibilidadViewModel> AgregarDisponibilidadAsync(DisponibilidadCrearViewModel modelo)
        {
            // Regla de negocio: la hora de fin debe ser posterior a la hora de inicio
            if (modelo.HoraFin <= modelo.HoraInicio)
                throw new InvalidOperationException("La hora de fin debe ser posterior a la hora de inicio.");

            var medico = await _repo.ObtenerPorIdAsync(modelo.MedicoId)
                ?? throw new InvalidOperationException("El médico no existe.");

            var hospitalId = modelo.HospitalId ?? medico.HospitalId;
            if (hospitalId <= 0)
                throw new InvalidOperationException("Debe indicar un hospital válido para la disponibilidad.");

            var hospitalesAsignados = await _repo.ObtenerHospitalesAsignadosAsync(modelo.MedicoId);
            if (!hospitalesAsignados.Any(h => h.Id == hospitalId))
                throw new InvalidOperationException("El hospital no está asignado al médico.");

            var disponibilidad = new DoctorAvailability
            {
                DoctorId  = modelo.MedicoId,
                HospitalId = hospitalId,
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

        /// <inheritdoc/>
        public async Task<IEnumerable<HospitalAsignadoViewModel>> ObtenerHospitalesAsignadosAsync(int medicoId)
        {
            var medico = await _repo.ObtenerPorIdAsync(medicoId)
                ?? throw new KeyNotFoundException($"Médico con id {medicoId} no encontrado.");

            var hospitales = await _repo.ObtenerHospitalesAsignadosAsync(medicoId);

            // Compatibilidad con medicos existentes que solo tengan HospitalId en la entidad.
            if (!hospitales.Any() && medico.HospitalId > 0)
            {
                await _repo.ActualizarHospitalesAsignadosAsync(medicoId, new[] { medico.HospitalId });
                hospitales = await _repo.ObtenerHospitalesAsignadosAsync(medicoId);
            }

            return hospitales.Select(h => new HospitalAsignadoViewModel
            {
                HospitalId = h.Id,
                NombreHospital = h.Name
            });
        }

        /// <inheritdoc/>
        public async Task ActualizarHospitalesAsignadosAsync(int medicoId, HospitalesAsignadosActualizarViewModel modelo)
        {
            var medico = await _repo.ObtenerPorIdAsync(medicoId)
                ?? throw new KeyNotFoundException($"Médico con id {medicoId} no encontrado.");

            var hospitalesIds = modelo.HospitalesIds.Distinct().ToList();
            if (!hospitalesIds.Any())
                throw new InvalidOperationException("El médico debe tener al menos un hospital asignado.");

            foreach (var hospitalId in hospitalesIds)
            {
                if (!await _repo.ExisteHospitalAsync(hospitalId))
                    throw new InvalidOperationException($"Hospital con id {hospitalId} no existe.");
            }

            await _repo.ActualizarHospitalesAsignadosAsync(medicoId, hospitalesIds);

            // Conserva un hospital principal para compatibilidad con módulos existentes.
            medico.HospitalId = hospitalesIds.First();
            await _repo.ActualizarAsync(medico);
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
            HospitalId   = d.HospitalId,
            NombreHospital = d.Hospital?.Name ?? string.Empty,
            DiaSemana    = d.DayOfWeek,
            HoraInicio   = d.StartTime,
            HoraFin      = d.EndTime
        };
    }
}
