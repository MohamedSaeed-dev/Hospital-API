using Microsoft.SqlServer.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace HospitalAPI.Models.DataModels
{
    public class Billing
    {
        [Key]
        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public double Amount { get; set; }
        public DateTime DateTime { get; set; } = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"), "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
        public BillingStatus Status { get; set; } = BillingStatus.UnPaid;
        
        public Appointment Appointment { get; set; }
    }
    public enum BillingStatus
    {
        Paid = 1,
        UnPaid
    }
}
