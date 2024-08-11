using HospitalAPI.Features.Mail.Service;
using HospitalAPI.Features.Redis.Service;
using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HospitalAPI.Features.Utils.Repository
{
    public class UtilitiesRepository : IUtilitiesService
    {
        private readonly HttpClient _httpClient;
        private readonly MyDbContext _db;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _http;
        private readonly IResponseStatus _response;
        private readonly IRedisService _redis;
        private const string chars = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";
        public UtilitiesRepository(IConfiguration config, IHttpContextAccessor http, IResponseStatus response, IRedisService redis, MyDbContext db, IMailService mailService, HttpClient httpClient)
        {
            _config = config;
            _http = http;
            _response = response;
            _redis = redis;
            _db = db;
            _mailService = mailService;
            _httpClient = httpClient;
        }
        public string GenerateCode()
        {
            StringBuilder code = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                code.Append(chars[Random.Shared.Next(0, chars.Length)]);
            }
            return code.ToString();
        }

        public string GenerateOTP()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public bool IsEmail(string email)
        {
            var emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailRegex);
        }

        public string ShortenEmail(string email)
        {
            // power123@gmail.com
            // pow*****@gmail.com
            var halfEmail = email.Split("@");
            var length = halfEmail[0].Length;
            var firstThree = halfEmail[0].Substring(0, 3);
            var newEmail = firstThree.PadRight(length, '*');
            return newEmail + halfEmail[1];
        }
        public async Task<ResponseStatus> SendEmail(string email, string subject, string body)
        {
            try
            {
                var isEmailWork = await CheckEmail(email);
                if (isEmailWork.StatusCode != 200) return _response.Custom(isEmailWork.StatusCode, isEmailWork.Message!);
                EmailCheckViewModel emailCheck = (EmailCheckViewModel)isEmailWork.Message!;
                if (!emailCheck.SmtpCheck) return _response.BadRequest("Check correctness of your Email");
                MailDTO mailDTO = new MailDTO
                {
                    ToEmail = email,
                    Subject = subject,
                    Body = body
                };
                await _mailService.SendMail(mailDTO);
                return _response.Ok($"The Email is Sent Successfully to {ShortenEmail(email)}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ResponseStatus> SendOTP(string email)
        {
            try
            {
                var user = await _db.Users.SingleOrDefaultAsync(x => x.Email == email);
                if (user != null && user.IsVerified) return _response.BadRequest("User is already verified");
                var otp = GenerateOTP();
                await _redis.Add($"{email}_OTP", otp, TimeSpan.FromMinutes(10));
                var subject = "Email Verification - Hospital System";
                var body = $"Here is the OTP for your Email Verification: <br> <strong>{otp}</strong> <br> Don't share it with anyone. <br> if you're not who did that action, just ignore it";
                var response = await SendEmail(email, subject, body);
                if (response.StatusCode != 200) return _response.Custom(response.StatusCode, response.Message!);
                return _response.Custom(response.StatusCode, response.Message!);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<ResponseStatus> CheckEmail(string email)
        {
            try
            {
                var messageRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.apilayer.com/email_verification/check?email={email}");
                messageRequest.Headers.Add("apiKey", _config["EmailCheckAPI"]);
                var send = await _httpClient.SendAsync(messageRequest);
                if (!send.IsSuccessStatusCode) return _response.Custom((int)send.StatusCode, "Something went wrong with Email Check");
                var content = await send.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<EmailCheckViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true,
                });
                return _response.Ok(json!);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }

}
