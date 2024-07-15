using HospitalAPI.Models.DataModels;

namespace HospitalAPI.Models.DTOs
{
    public class PatientDTO : Person
    {
        public int DoctorId  { get; set; }
    }
}
