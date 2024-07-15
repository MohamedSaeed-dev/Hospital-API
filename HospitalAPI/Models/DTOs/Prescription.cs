using HospitalAPI.Models.DataModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HospitalAPI.Models.DTOs
{
    public class PrescriptionDTO
    {
        public int? AppointmentId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        [MaxLength(100)]
        [Required]
        public string? Medication { get; set; } = string.Empty;
    }
}
