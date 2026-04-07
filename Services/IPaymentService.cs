using MedSync_API.Models.ViewModels;

namespace MedSync_API.Services
{
    /// <summary>
    /// Contrato del servicio de pagos.
    /// Define las operaciones de negocio del módulo financiero (RF16, RF19).
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>Obtiene los pagos de un hospital ordenados por fecha.</summary>
        Task<IEnumerable<PagoViewModel>> ObtenerPorHospitalAsync(int hospitalId);

        /// <summary>Registra el pago de una cita médica (RF16).</summary>
        Task<PagoViewModel> CrearAsync(PagoCrearViewModel modelo);

        /// <summary>Actualiza el estado de un pago existente.</summary>
        Task CambiarEstadoAsync(int id, string estado);

        /// <summary>
        /// Elimina un pago registrado por error.
        /// Solo permite eliminar pagos en estado "Pending".
        /// </summary>
        Task EliminarAsync(int id);
    }
}
