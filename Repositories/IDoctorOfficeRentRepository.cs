using MedSync_API.Models;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Repositorio específico para cobros de consultorio a médicos.
    /// </summary>
    public interface IDoctorOfficeRentRepository : IGenericRepository<DoctorOfficeRent>
    {
        Task<IEnumerable<DoctorOfficeRent>> ObtenerPorHospitalAsync(int hospitalId);
        Task<IEnumerable<DoctorOfficeRent>> ObtenerPorDoctorAsync(int doctorId);
    }
}