using HospitalAPI.Features.Mail.Service;
using HospitalAPI.Features.Redis.Service;
using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace HospitalAPI.Features.Utils.Repository
{
    public class UtilitiesRepository : IUtilitiesService
    {
        private readonly MyDbContext _db;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _http;
        private readonly IResponseStatus _response;
        private readonly IRedisService _redis;
        private const string chars = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";
        public UtilitiesRepository(IConfiguration config, IHttpContextAccessor http, IResponseStatus response, IRedisService redis, MyDbContext db, IMailService mailService)
        {
            _config = config;
            _http = http;
            _response = response;
            _redis = redis;
            _db = db;
            _mailService = mailService;
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
                var user = await _db.Users.SingleOrDefaultAsync(x => x.Email == email);
                if (user == null) return _response.BadRequest("User is not exist");
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
                if (user == null) return _response.BadRequest("User is not exist");
                if (user.IsVerified) return _response.BadRequest("User is already verified");
                var otp = GenerateOTP();
                await _redis.Add(email, otp, TimeSpan.FromMinutes(5));
                var subject = "Email Verification - Hospital System";
                var body = $"Here is the OTP for your Email Verification: {otp}, Don't share it with anyone.";
                await SendEmail(email, subject, body);
                return _response.Ok("The OTP is sent Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
