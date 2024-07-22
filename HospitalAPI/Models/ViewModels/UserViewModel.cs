using HospitalAPI.Models.DataModels;

namespace HospitalAPI.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public Role Role { get; set; }
    }
}
