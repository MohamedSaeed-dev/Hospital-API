namespace HospitalAPI.Models.DataModels
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public Role Role { get; set; }
    }
    public enum Role
    {
        Admin = 1,
        Receptionist
    }
}
