//using System.ComponentModel.DataAnnotations.Data
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Klucz złożony dla tabeli many-to-many
            modelBuilder.Entity<PrescriptionMedicament>()
                .HasKey(pm => new { pm.IdMedicament, pm.IdPrescription });

            // Relacje
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(p => p.Presciptions)
                .HasForeignKey(p => p.IdPatient);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Prescription)
                .HasForeignKey(p => p.IdDoctor);

            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdPrescription);

            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Medicament)
                .WithMany(m => m.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdMedicament);

            // (Opcjonalnie) Przykładowe dane
            modelBuilder.Entity<Doctor>().HasData(new Doctor
            {
                IdDoctor = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@clinic.com"
            });

            modelBuilder.Entity<Patient>().HasData(new Patient
            {
                IdPatient = 1,
                FirstName = "Anna",
                LastName = "Nowak",
                //Birthdata = new DateTime(1990, 1, 1),
            });

            modelBuilder.Entity<Medicament>().HasData(new Medicament
            {
                IdMedicament = 1,
                Name = "Paracetamol",
                Description = "Painkiller",
                Type = "Tablet"
            });

            modelBuilder.Entity<Prescription>().HasData(new Prescription
            {
                IdPrescription = 1,
                IdDoctor = 1,
                IdPatient = 1,
                Date = new DateTime(2025, 1, 1),
                DueDate = new DateTime(2025, 1, 10)
            });

            modelBuilder.Entity<PrescriptionMedicament>().HasData(new PrescriptionMedicament
            {
                IdPrescription = 1,
                IdMedicament = 1,
                Dose = 2,
                Details = "Take after meal"
            });
        }
    }
}
