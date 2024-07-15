using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalAPI.Models.DataModels
{
    public class Billing
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }

        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
        public BillingStatus Status { get; set; } = BillingStatus.UnPaid;
        
        public Appointment Appointment { get; set; }
    }
    public enum BillingStatus
    {
        Paid = 1,
        UnPaid
    }
}
