using HospitalAPI.Models.DataModels;

namespace HospitalAPI.Models.DTOs
{
    public class AppointmentDTO
    {
        public int? DoctorPatientId { get; set; }

        public DateTime? DateTime { get; set; }
        public AppointmentStatus? Status { get; set; } = AppointmentStatus.Scheduled;

    }
}
