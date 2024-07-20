using HospitalAPI.Models.DataModels;

namespace HospitalAPI.Models.ViewModels
{
    public class PateintAppointmentViewModel
    {
        public string PatientName { get; set; } = null!;
        public Gender PatientGender { get; set; }
        public string DoctorName { get; set; } = null!;
        public string DoctorDepartment { get; set; } = null!;
        public Gender DoctorGender { get; set; }
        public DateTime AppointmentDate { get; set; }
        public double BillingAmount { get; set; }
        public BillingStatus BillingStatus { get; set; }
    }
}
