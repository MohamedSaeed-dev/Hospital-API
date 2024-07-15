using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalAPI.Models.DataModels
{
    public class Prescription
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }

        [MaxLength(100)]
        public string Medication { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public Appointment Appointment { get; set; }
    }
}
