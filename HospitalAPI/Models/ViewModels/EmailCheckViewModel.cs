using System.Text.Json.Serialization;

namespace HospitalAPI.Models.ViewModels
{
    public class EmailCheckViewModel
    {
        [JsonPropertyName("smtp_check")]
        public bool SmtpCheck { get; set; }
        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}
