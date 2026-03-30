using MedSync_API.Models;
using MedSync_API.Models.ViewModels;
using MedSync_API.Repositories;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de hospitales.
    /// Aplica las reglas de negocio del módulo de administración de centros médicos (RF20).
    /// </summary>
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _repo;

        public HospitalService(IHospitalRepository repo)
        {
            _repo = repo;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<HospitalViewModel>> ObtenerTodosAsync()
        {
            var hospitales = await _repo.ObtenerTodosAsync();
            return hospitales.Select(MapearAViewModel);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<HospitalViewModel>> ObtenerActivosAsync()
        {
            var hospitales = await _repo.ObtenerActivosAsync();
            return hospitales.Select(MapearAViewModel);
        }

        /// <inheritdoc/>
        public async Task<HospitalViewModel?> ObtenerPorIdAsync(int id)
        {
            var hospital = await _repo.ObtenerPorIdAsync(id);
            return hospital is null ? null : MapearAViewModel(hospital);
        }

        /// <inheritdoc/>
        public async Task<HospitalViewModel> CrearAsync(HospitalCrearViewModel modelo)
        {
            // Regla de negocio: el RNC debe ser único en el sistema
            if (await _repo.ExisteRncAsync(modelo.Rnc))
                throw new InvalidOperationException($"Ya existe un hospital registrado con el RNC '{modelo.Rnc}'.");

            var hospital = new Hospital
            {
                Name    = modelo.Nombre,
                Rnc     = modelo.Rnc,
                Address = modelo.Direccion,
                Phone   = modelo.Telefono
            };

            await _repo.AgregarAsync(hospital);
            return MapearAViewModel(hospital);
        }

        /// <inheritdoc/>
        public async Task ActualizarAsync(int id, HospitalActualizarViewModel modelo)
        {
            var hospital = await _repo.ObtenerPorIdAsync(id)
                ?? throw new KeyNotFoundException($"Hospital con id {id} no encontrado.");

            hospital.Name     = modelo.Nombre;
            hospital.Rnc      = modelo.Rnc;
            hospital.Address  = modelo.Direccion;
            hospital.Phone    = modelo.Telefono;
            hospital.IsActive = modelo.EstaActivo;

            await _repo.ActualizarAsync(hospital);
        }

        // ─── Mapeo de dominio a ViewModel ────────────────────────────────────

        /// <summary>Convierte una entidad Hospital a su ViewModel de respuesta.</summary>
        private static HospitalViewModel MapearAViewModel(Hospital h) => new()
        {
            Id        = h.Id,
            Nombre    = h.Name,
            Rnc       = h.Rnc,
            Direccion = h.Address,
            Telefono  = h.Phone,
            EstaActivo = h.IsActive,
            CreadoEn  = h.CreatedAt
        };
    }
}
