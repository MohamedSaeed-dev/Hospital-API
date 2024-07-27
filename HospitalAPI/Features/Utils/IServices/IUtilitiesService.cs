using HospitalAPI.Features.Mail.Service;

namespace HospitalAPI.Features.Utils.IServices
{
    public interface IUtilitiesService : IEmailService
    {
        string GenerateOTP();
        string GenerateCode();
    }
}
