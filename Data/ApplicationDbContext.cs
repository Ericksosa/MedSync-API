using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MedSync_API.Models;

namespace MedSync_API.Data
{
    /// <summary>
    /// Contexto principal de Entity Framework Core para MedSync.
    /// Extiende IdentityDbContext para integrar las tablas de autenticación de ASP.NET Identity
    /// (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.) junto con las entidades del dominio.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // ─── Entidades del dominio ────────────────────────────────────────────

        /// <summary>Tabla de citas médicas entre pacientes y médicos (RF05-RF10).</summary>
        public DbSet<Appointment> Appointments { get; set; }

        /// <summary>Tabla de médicos registrados en el sistema (RF22).</summary>
        public DbSet<Doctor> Doctors { get; set; }

        /// <summary>Tabla de hospitales o centros médicos (RF20).</summary>
        public DbSet<Hospital> Hospitals { get; set; }

        /// <summary>Tabla de pacientes registrados (RF01).</summary>
        public DbSet<Patient> Patients { get; set; }

        /// <summary>Tabla de pagos por consultas médicas (RF16).</summary>
        public DbSet<Payment> Payments { get; set; }

        /// <summary>Tabla de especialidades médicas por hospital (RF21).</summary>
        public DbSet<Specialty> Specialties { get; set; }

        /// <summary>Tabla de expedientes médicos electrónicos de los pacientes (RF11).</summary>
        public DbSet<MedicalRecord> MedicalRecords { get; set; }

        /// <summary>Tabla de diagnósticos registrados por los médicos (RF12).</summary>
        public DbSet<Diagnosis> Diagnoses { get; set; }

        /// <summary>Tabla de recetas médicas emitidas (RF13).</summary>
        public DbSet<Prescription> Prescriptions { get; set; }

        /// <summary>Tabla de disponibilidad horaria de cada médico (RF23).</summary>
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }

        /// <summary>Tabla de cobros de consultorio a los médicos (RF17, RF18).</summary>
        public DbSet<DoctorOfficeRent> DoctorOfficeRents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primero configura las tablas de Identity (AspNetUsers, Roles, etc.)
            base.OnModelCreating(modelBuilder);

            // ─── Regla de integridad global ───────────────────────────────────
            // Evitar borrado en cascada en todas las relaciones del sistema.
            // Si se elimina un hospital o especialidad, los médicos y citas asociadas
            // no se borran automáticamente, protegiendo la integridad del historial clínico.
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // ─── Índices únicos ───────────────────────────────────────────────

            // El documento de identidad del paciente debe ser único en el sistema
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.DocumentId)
                .IsUnique();

            // El número de exequátur identifica de forma única a cada médico
            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.MedicalLicense)
                .IsUnique();

            // El RNC del hospital debe ser único
            modelBuilder.Entity<Hospital>()
                .HasIndex(h => h.Rnc)
                .IsUnique();

            // Un paciente solo puede tener un expediente médico (relación 1 a 1)
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.MedicalRecord)
                .WithOne(e => e.Patient)
                .HasForeignKey<MedicalRecord>(e => e.PatientId);
        }
    }
}
