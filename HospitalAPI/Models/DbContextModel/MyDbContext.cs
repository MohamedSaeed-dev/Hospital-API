using HospitalAPI.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace HospitalAPI.Models.DbContextModel
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            
        }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<DoctorPatient> DoctorPatients { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>()
                .HasData(
                new Department { Id = 1, Name = "Eyes" },
                new Department { Id = 2, Name = "Dentest" },
                new Department { Id = 3, Name = "Bones" }
                );
            modelBuilder.Entity<Doctor>()
                .HasData(
                new Doctor { Id = 1, Address = "Mukalla", BonusRate = 2.5, DepartmentId = 1, Email = "ali@gmail.com", FullName= "ali", Gender = Gender.Male, Phone = "123456789", Salary = 50000, Status = DoctorStatus.Available },
                new Doctor { Id = 2, Address = "Mukalla", BonusRate = 1.5, DepartmentId = 2, Email = "huda@gmail.com", FullName = "huda", Gender = Gender.Female, Phone = "987654321", Salary = 30000, Status = DoctorStatus.Available },
                new Doctor { Id = 3, Address = "Mukalla", BonusRate = 2, DepartmentId = 3, Email = "salem@gmail.com", FullName = "salem", Gender = Gender.Male, Phone = "567894321", Salary = 45000, Status = DoctorStatus.Available },
                new Doctor { Id = 4, Address = "Mukalla", BonusRate = 4, DepartmentId = 1, Email = "zainab@gmail.com", FullName = "zainab", Gender = Gender.Female, Phone = "299388477", Salary = 39000, Status = DoctorStatus.UnAvailabe }
                );

            modelBuilder.Entity<Patient>()
                .HasData(
                new Patient { Id = 1, Address = "Mukalla", Email = "yasser@gamil.com", FullName = "yasser", Gender = Gender.Male, Phone = "192837465" },
                new Patient { Id = 2, Address = "Mukalla", Email = "khadija@gamil.com", FullName = "khadija", Gender = Gender.Female, Phone = "654738291" },
                new Patient { Id = 3, Address = "Mukalla", Email = "ahmed@gamil.com", FullName = "ahmed", Gender = Gender.Male, Phone = "135792468" }
                );

            modelBuilder.Entity<DoctorPatient>()
                .HasData(
                new DoctorPatient { Id = 1, DoctorId = 1, PatientId = 3 },
                new DoctorPatient { Id = 2, DoctorId = 2, PatientId = 1 },
                new DoctorPatient { Id = 3, DoctorId = 3, PatientId = 2 }
                );

            modelBuilder.Entity<Appointment>()
                .HasData(
                new Appointment { Id = 1, DoctorPatientId = 1, Status = AppointmentStatus.Scheduled, DateTime = DateTime.ParseExact("17/07/2024 03:00 PM", "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture) },
                new Appointment { Id = 2, DoctorPatientId = 2, Status = AppointmentStatus.Completed, DateTime = DateTime.ParseExact("17/07/2024 01:30 PM", "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture) },
                new Appointment { Id = 3, DoctorPatientId = 3, Status = AppointmentStatus.Canceled, DateTime = DateTime.ParseExact("17/07/2024 02:00 PM", "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture) }
                );

            modelBuilder.Entity<Billing>()
                .HasData(
                new Billing { AppointmentId = 1, Amount = 1000, Status = BillingStatus.Paid },
                new Billing { AppointmentId = 2, Amount = 2500, Status = BillingStatus.Paid },
                new Billing { AppointmentId = 3, Amount = 3000, Status = BillingStatus.UnPaid }
                );

            modelBuilder.Entity<Prescription>()
                .HasData(
                new Prescription { AppointmentId = 1, Medication = "Takes everyday", EndDate = DateOnly.FromDateTime(DateTime.ParseExact("17/08/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture)) },
                new Prescription { AppointmentId = 2, Medication = "Takes one pill before sleep", EndDate = DateOnly.FromDateTime(DateTime.ParseExact("01/07/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture)) }
                );

            // One to Many
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Department)
                .WithMany()
                .HasForeignKey(d => d.DepartmentId);

            modelBuilder.Entity<DoctorPatient>()
                .HasOne(dp => dp.Doctor)
                .WithMany(d => d.DoctorPatients)
                .HasForeignKey(dp => dp.DoctorId);

            modelBuilder.Entity<DoctorPatient>()
                .HasOne(dp => dp.Patient)
                .WithMany(p => p.DoctorPatients)
                .HasForeignKey(dp => dp.PatientId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.DoctorPatient)
                .WithMany(dp => dp.Appointments)
                .HasForeignKey(a => a.DoctorPatientId);

        }
    }
}
