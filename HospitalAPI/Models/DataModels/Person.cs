using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models.DataModels
{
    public class Person
    {
        [MaxLength(30)]
        public string? FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        [MaxLength(10)]
        public string? Phone { get; set; } = string.Empty;
        [MaxLength(30)]
        public string? Address { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
    }

    public enum Gender
    {
        Male = 1,
        Female
    }
}
