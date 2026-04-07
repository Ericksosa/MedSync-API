using MedSync_API.Models;
using MedSync_API.Models.ViewModels;
using MedSync_API.Repositories;

namespace MedSync_API.Services
{
    /// <summary>
    /// Implementación del servicio de cobros de consultorio a médicos.
    /// </summary>
    public class CobroService : ICobroService
    {
        private static readonly string[] EstadosValidos = ["Pendiente", "Pagado"];
        private static readonly string[] PeriodosValidos =
        [
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        ];

        private readonly IDoctorOfficeRentRepository _cobroRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IHospitalRepository _hospitalRepo;
        private readonly IPaymentRepository _paymentRepo;

        public CobroService(
            IDoctorOfficeRentRepository cobroRepo,
            IDoctorRepository doctorRepo,
            IHospitalRepository hospitalRepo,
            IPaymentRepository paymentRepo)
        {
            _cobroRepo = cobroRepo;
            _doctorRepo = doctorRepo;
            _hospitalRepo = hospitalRepo;
            _paymentRepo = paymentRepo;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CobroViewModel>> ObtenerPorHospitalAsync(int hospitalId)
        {
            var cobros = await _cobroRepo.ObtenerPorHospitalAsync(hospitalId);
            return cobros.Select(MapearACobroViewModel);
        }

        /// <inheritdoc/>
        public async Task<CobroViewModel> CrearAsync(CobroCrearViewModel modelo)
        {
            if (!EstadosValidos.Contains(modelo.Estado))
                throw new InvalidOperationException($"Estado '{modelo.Estado}' no válido. Use: {string.Join(", ", EstadosValidos)}.");

            if (!PeriodosValidos.Contains(modelo.Periodo))
                throw new InvalidOperationException($"Período '{modelo.Periodo}' no válido. Use: {string.Join(", ", PeriodosValidos)}.");

            var doctor = await _doctorRepo.ObtenerPorIdAsync(modelo.MedicoId)
                ?? throw new KeyNotFoundException($"Médico con id {modelo.MedicoId} no encontrado.");

            var hospital = await _hospitalRepo.ObtenerPorIdAsync(modelo.HospitalId)
                ?? throw new KeyNotFoundException($"Hospital con id {modelo.HospitalId} no encontrado.");

            var cobro = new DoctorOfficeRent
            {
                DoctorId = modelo.MedicoId,
                HospitalId = modelo.HospitalId,
                Period = modelo.Periodo,
                Amount = modelo.Monto,
                Status = modelo.Estado,
                DueDate = DateTime.UtcNow,
                PaymentDate = modelo.Estado == "Pagado" ? DateTime.UtcNow : null,
                Doctor = doctor,
                Hospital = hospital
            };

            await _cobroRepo.AgregarAsync(cobro);
            return MapearACobroViewModel(cobro);
        }

        /// <inheritdoc/>
        public async Task CambiarEstadoAsync(int id, string estado)
        {
            if (!EstadosValidos.Contains(estado))
                throw new InvalidOperationException($"Estado '{estado}' no válido. Use: {string.Join(", ", EstadosValidos)}.");

            var cobro = await _cobroRepo.ObtenerPorIdAsync(id)
                ?? throw new KeyNotFoundException($"Cobro con id {id} no encontrado.");

            cobro.Status = estado;
            cobro.PaymentDate = estado == "Pagado" ? DateTime.UtcNow : null;

            await _cobroRepo.ActualizarAsync(cobro);
        }

        /// <inheritdoc/>
        public async Task<BalanceMedicoViewModel> ObtenerBalanceDoctorAsync(int doctorId)
        {
            var doctor = await _doctorRepo.ObtenerPorIdAsync(doctorId)
                ?? throw new KeyNotFoundException($"Médico con id {doctorId} no encontrado.");

            var cobros = await _cobroRepo.ObtenerPorDoctorAsync(doctorId);
            var totalCobrado = cobros.Sum(c => c.Amount);
            var totalPagado = cobros.Where(c => c.Status == "Pagado").Sum(c => c.Amount);
            var totalGenerado = await _paymentRepo.ObtenerTotalGeneradoPorDoctorAsync(doctorId);

            return new BalanceMedicoViewModel
            {
                NombreMedico = $"{doctor.FirstName} {doctor.LastName}".Trim(),
                TotalGenerado = totalGenerado,
                TotalCobrado = totalCobrado,
                TotalPagado = totalPagado
            };
        }

        private static CobroViewModel MapearACobroViewModel(DoctorOfficeRent c) => new()
        {
            Id = c.Id,
            MedicoId = c.DoctorId,
            NombreMedico = c.Doctor is null ? string.Empty : $"{c.Doctor.FirstName} {c.Doctor.LastName}".Trim(),
            HospitalId = c.HospitalId,
            NombreHospital = c.Hospital?.Name ?? string.Empty,
            Periodo = c.Period,
            Monto = c.Amount,
            Estado = c.Status,
            FechaEmision = c.DueDate,
            FechaPago = c.PaymentDate
        };
    }
}