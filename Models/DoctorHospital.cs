using System.ComponentModel.DataAnnotations.Schema;

namespace MedSync_API.Models
{
    /// <summary>
    /// Relacion muchos-a-muchos entre medicos y hospitales.
    /// Permite que un medico pueda trabajar en varias sedes.
    /// </summary>
    [Table("DoctorHospitals")]
    public class DoctorHospital
    {
        public int DoctorId { get; set; }
        public int HospitalId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor? Doctor { get; set; }

        [ForeignKey(nameof(HospitalId))]
        public Hospital? Hospital { get; set; }
    }
}
