using HospitalAPI.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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

            // One to One
            modelBuilder.Entity<Billing>()
                .HasOne(x => x.Appointment)
                .WithOne()
                .HasForeignKey<Billing>(x => x.AppointmentId);

            modelBuilder.Entity<Prescription>()
                .HasOne(x => x.Appointment)
                .WithOne()
                .HasForeignKey<Prescription>(x => x.AppointmentId);


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
