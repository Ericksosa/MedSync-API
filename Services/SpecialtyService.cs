using MedSync_API.Models;
using MedSync_API.Models.ViewModels;
using MedSync_API.Repositories;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de especialidades médicas.
    /// Aplica las reglas de negocio del módulo de administración (RF21).
    /// </summary>
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ISpecialtyRepository _repo;

        public SpecialtyService(ISpecialtyRepository repo)
        {
            _repo = repo;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EspecialidadViewModel>> ObtenerPorHospitalAsync(int hospitalId)
        {
            var especialidades = await _repo.ObtenerPorHospitalAsync(hospitalId);
            return especialidades.Select(MapearAViewModel);
        }

        /// <inheritdoc/>
        public async Task<EspecialidadViewModel> CrearAsync(EspecialidadCrearViewModel modelo)
        {
            // Regla de negocio: no puede haber dos especialidades con el mismo nombre en el mismo hospital
            if (await _repo.ExisteEnHospitalAsync(modelo.Nombre, modelo.HospitalId))
                throw new InvalidOperationException(
                    $"Ya existe la especialidad '{modelo.Nombre}' en este hospital.");

            var especialidad = new Specialty
            {
                Name        = modelo.Nombre,
                Description = modelo.Descripcion,
                HospitalId  = modelo.HospitalId
            };

            await _repo.AgregarAsync(especialidad);
            return MapearAViewModel(especialidad);
        }

        // ─── Mapeo de dominio a ViewModel ────────────────────────────────────

        private static EspecialidadViewModel MapearAViewModel(Specialty s) => new()
        {
            Id             = s.Id,
            Nombre         = s.Name,
            Descripcion    = s.Description,
            HospitalId     = s.HospitalId,
            NombreHospital = s.Hospital?.Name ?? string.Empty
        };
    }
}
