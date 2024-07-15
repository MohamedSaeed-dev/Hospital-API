using HospitalAPI.Models.DataModels;

namespace HospitalAPI.Models.DTOs
{
    public class BillingDTO
    {
        public int? AppointmentId { get; set; }

        public double? Amount { get; set; }
        public DateTime? DateTime { get; set; }
        public BillingStatus? Status { get; set; } = BillingStatus.UnPaid;
    }
}
