namespace HospitalAPI.Features.Mail.Service
{
    public interface IEmailService
    {
        bool IsEmail(string email);
        string ShortenEmail(string email);
    }
}
