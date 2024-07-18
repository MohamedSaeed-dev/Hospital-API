using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models.DataModels
{
    public class Doctor : Person
    {
        public int Id { get; set; }
        public int? DepartmentId { get; set; }
        [Required]
        [Range(1000, 1000000)]
        public double Salary { get; set; }
        public double BonusRate { get; set; }
        public DoctorStatus Status { get; set; }

        public Department Department { get; set; }
        public ICollection<DoctorPatient> DoctorPatients { get; set; }

    }
    public enum DoctorStatus
    {
        Available = 1,
        UnAvailabe,
        Leave
    }
}
