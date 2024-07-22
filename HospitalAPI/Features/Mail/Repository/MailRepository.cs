using HospitalAPI.Features.Mail;
using HospitalAPI.Models.DTOs;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using HospitalAPI.Features.Mail.Service;


namespace HospitalAPI.Features.Mail.Repository
{
    public class MailRepository : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailRepository(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendMail(MailDTO mailDTO)
        {
            using (var email = new MimeMessage())
            {
                email.Sender = MailboxAddress.Parse(_mailSettings.Email);
                email.Subject = mailDTO.Subject;
                email.To.Add(MailboxAddress.Parse(mailDTO.ToEmail));

                var builder = new BodyBuilder();
                builder.HtmlBody = mailDTO.Body;
                email.Body = builder.ToMessageBody();
                email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }
            }
        }
    }
}
