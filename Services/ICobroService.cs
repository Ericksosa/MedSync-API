using MedSync_API.Models.ViewModels;

namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de cobros de consultorio (RF17, RF18).
    /// </summary>
    public interface ICobroService
    {
        Task<IEnumerable<CobroViewModel>> ObtenerPorHospitalAsync(int hospitalId);
        Task<CobroViewModel> CrearAsync(CobroCrearViewModel modelo);
        Task CambiarEstadoAsync(int id, string estado);
        Task<BalanceMedicoViewModel> ObtenerBalanceDoctorAsync(int doctorId);
    }
}