using MedSync_API.Models;
using MedSync_API.Models.ViewModels;
using MedSync_API.Repositories;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de pagos.
    /// Aplica las reglas de negocio del módulo financiero de citas (RF16, RF19).
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;

        /// <summary>Métodos de pago válidos para una consulta médica.</summary>
        private static readonly string[] MetodosPagoValidos = ["Card", "Cash", "Insurance"];

        /// <summary>Estados válidos de un pago.</summary>
        private static readonly string[] EstadosPagoValidos = ["Completed", "Pending", "Refunded"];

        public PaymentService(IPaymentRepository repo)
        {
            _repo = repo;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PagoViewModel>> ObtenerPorHospitalAsync(int hospitalId)
        {
            var pagos = await _repo.ObtenerPorHospitalAsync(hospitalId);
            return pagos.Select(MapearAViewModel);
        }

        /// <inheritdoc/>
        public async Task<PagoViewModel> CrearAsync(PagoCrearViewModel modelo)
        {
            // Regla de negocio: solo se acepta un pago por cita (relación 1 a 1)
            var pagoExistente = await _repo.ObtenerPorCitaAsync(modelo.CitaId);
            if (pagoExistente is not null)
                throw new InvalidOperationException("Esta cita ya tiene un pago registrado.");

            // Valida que el método de pago sea uno de los definidos
            if (!MetodosPagoValidos.Contains(modelo.MetodoPago))
                throw new InvalidOperationException(
                    $"Método de pago '{modelo.MetodoPago}' no válido. Use: {string.Join(", ", MetodosPagoValidos)}.");

            // Valida que el estado sea uno de los definidos
            if (!EstadosPagoValidos.Contains(modelo.Estado))
                throw new InvalidOperationException(
                    $"Estado '{modelo.Estado}' no válido. Use: {string.Join(", ", EstadosPagoValidos)}.");

            var pago = new Payment
            {
                AppointmentId = modelo.CitaId,
                Amount        = modelo.Monto,
                PaymentMethod = modelo.MetodoPago,
                Status        = modelo.Estado
            };

            await _repo.AgregarAsync(pago);
            return MapearAViewModel(pago);
        }

        /// <inheritdoc/>
        public async Task EliminarAsync(int id)
        {
            var pago = await _repo.ObtenerPorIdAsync(id)
                ?? throw new KeyNotFoundException($"Pago con id {id} no encontrado.");

            // Regla de negocio: solo se pueden eliminar pagos en estado Pendiente
            // Los pagos completados requieren un proceso de reembolso
            if (pago.Status == "Completed")
                throw new InvalidOperationException("No se puede eliminar un pago completado. Genere un reembolso.");

            await _repo.EliminarAsync(pago);
        }

        // ─── Mapeo de dominio a ViewModel ────────────────────────────────────

        private static PagoViewModel MapearAViewModel(Payment p) => new()
        {
            Id         = p.Id,
            CitaId     = p.AppointmentId,
            Monto      = p.Amount,
            MetodoPago = p.PaymentMethod,
            Estado     = p.Status,
            FechaPago  = p.PaymentDate
        };
    }
}
