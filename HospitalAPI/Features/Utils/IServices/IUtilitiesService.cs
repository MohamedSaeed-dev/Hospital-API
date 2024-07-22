using HospitalAPI.Features.Mail.Service;
using HospitalAPI.Models.DataModels;

namespace HospitalAPI.Features.Utils.IServices
{
    public interface IUtilitiesService : IEmailService
    {
        string GenerateOTP();
        string GenerateCode();
        string GenerateToken(User user, string key, DateTime time);
    }
}
