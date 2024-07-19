using HospitalAPI.Models.DataModels;
using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models.DTOs
{
    public class DoctorDTO : Person
    {
        public int? DepartmentId { get; set; }
        public DoctorStatus? Status { get; set; }
        public double? Salary { get; set; }
        public double? BonusRate { get; set; }
    }
}
