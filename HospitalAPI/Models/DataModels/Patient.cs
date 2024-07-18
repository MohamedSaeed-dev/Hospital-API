namespace HospitalAPI.Models.DataModels
{
    public class Patient : Person
    {
        public int Id { get; set; }
        public ICollection<DoctorPatient> DoctorPatients { get; set; }
    }
}
