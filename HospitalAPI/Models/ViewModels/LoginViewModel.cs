namespace HospitalAPI.Models.ViewModels
{
    public class LoginViewModel
    {
        public string? Warning { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
