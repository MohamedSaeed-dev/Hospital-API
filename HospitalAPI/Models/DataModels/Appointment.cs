using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalAPI.Models.DataModels
{
    public class Appointment
    {
        public int Id { get; set; }
        public int DoctorPatientId { get; set; }
        public DateTime DateTime { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public DoctorPatient DoctorPatient { get; set; }
    }
    public enum AppointmentStatus
    {
        Scheduled = 1,
        Completed,
        Canceled
    }
}
