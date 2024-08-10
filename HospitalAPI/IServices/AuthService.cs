using HospitalAPI.Features;
using HospitalAPI.Features.Mail.Service;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;

namespace HospitalAPI.Services
{
    public interface IAuthService
    {
        Task<ResponseStatus> SignUp(UserSignUp user);
        Task<ResponseStatus> Login(UserLogin user);
        Task<ResponseStatus> Logout(int Id);
        Task<ResponseStatus> VerifyEmail(string otp, string email);
        Task<ResponseStatus> ForgotPassword(string email);
        Task<ResponseStatus> ResetPassword(string email, string code, string newPassword);
    }
}
