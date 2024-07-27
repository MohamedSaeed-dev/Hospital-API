using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.ViewModels;

namespace HospitalAPI.Features.Utils.IServices
{
    public interface ITokenService
    {
        Task<ResponseStatus> RefreshToken(string email);
        string GenerateToken(User user, string key, DateTime time);
        UserViewModel? VerifyToken(string token, string key);
    }
}
