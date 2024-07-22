using HospitalAPI.Models.DTOs;

namespace HospitalAPI.Features.Mail.Service
{
    public interface IMailService
    {
        Task SendMail(MailDTO mailDTO);
    }
}
