namespace HospitalAPI.Features.Mail.Service
{
    public interface IEmailService
    {
        bool IsEmail(string email);
        string ShortenEmail(string email);
        Task<ResponseStatus> SendEmail(string email, string subject, string body);
    }
}
