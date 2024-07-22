using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models.DTOs
{
    public class MailDTO
    {
        [Required]
        public string ToEmail { get; set; } = string.Empty;
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string Body { get; set; } = string.Empty;
    }
}
