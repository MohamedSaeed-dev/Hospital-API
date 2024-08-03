using AutoMapper;
using HospitalAPI.Features;
using HospitalAPI.Features.Mail.Service;
using HospitalAPI.Features.Redis.Service;
using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Repositories
{
    public class AuthRepository : IAuthService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IMailService _mailService;
        private readonly IRedisService _redis;
        private readonly IUtilitiesService _utilities;
        private readonly IResponseStatus _response;
        private readonly IHttpContextAccessor _http;
        private readonly ITokenService _token;

        public AuthRepository(MyDbContext db, IMapper mapper, IConfiguration config, IMailService mailService, IRedisService redis, IUtilitiesService utilities, IResponseStatus response, IHttpContextAccessor http, ITokenService token)
        {
            _db = db;
            _mapper = mapper;
            _config = config;
            _mailService = mailService;
            _redis = redis;
            _utilities = utilities;
            _response = response;
            _http = http;
            _token = token;
        }
        public async Task<ResponseStatus> Login(UserLogin user)
        {
            try
            {
                var record = await _db.Users.SingleOrDefaultAsync(x => x.Email == user.Email);
                if (record == null) return _response.BadRequest("User is not exist");
                var verifyPassword = BCrypt.Net.BCrypt.Verify(user.Password, record.Password);
                if (!verifyPassword) return _response.BadRequest("Password is Incorrect");
                var refreshToken = _token.GenerateToken(record, "KeyRefreshToken", DateTime.Now.AddDays(1));
                await _redis.Add($"{user.Email}_refreshToken", refreshToken, TimeSpan.FromDays(7));
                string token = _token.GenerateToken(record, "KeyAccessToken", DateTime.Now.AddMinutes(1));
                return _response.Ok(token);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<ResponseStatus> SignUp(UserSignUp user)
        {
            try
            {
                var record = await _db.Users.SingleOrDefaultAsync(x => x.UserName == user.UserName);
                if (record != null) return _response.BadRequest("User is exist");
                var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Password = hashPassword;
                User newUser = _mapper.Map<User>(user);
                newUser.Role = Role.Receptionist;
                await _db.Users.AddAsync(newUser);
                await _db.SaveChangesAsync();
                var refreshToken = _token.GenerateToken(newUser, "KeyRefreshToken", DateTime.Now.AddDays(1));
                await _redis.Add($"{user.Email}_refreshToken", refreshToken, TimeSpan.FromDays(7));
                string token = _token.GenerateToken(newUser, "KeyAccessToken", DateTime.Now.AddMinutes(1));
                return _response.Ok(token);
            }
            catch (Exception)
            {
                throw;
            }
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
                return _response.Ok($"The Email is Sent Successfully to {_utilities.ShortenEmail(email)}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ResponseStatus> VerifyEmail(string otp, string email)
        {
            try
            {
                var user = await _db.Users.SingleOrDefaultAsync(x => x.Email == email);
                var containsKey = await _redis.ContainsKey(email);
                if (user == null || !containsKey) return _response.BadRequest("The email is Incorrect"); ;
                var storedOTP = await _redis.Get(email);
                if (otp != storedOTP) throw new Exception("The OTP is Incorrect");
                user.IsVerified = true;
                await _redis.Delete(email);
                await _db.SaveChangesAsync();
                return _response.Ok("The Email is Verified Successfully");
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
                var otp = _utilities.GenerateOTP();
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

        public async Task<ResponseStatus> ForgotPassword(string email)
        {
            try
            {
                var user = _db.Users.SingleOrDefault(x => x.Email == email);
                if (user == null) return _response.BadRequest("User is not exist");
                if (!user.IsVerified) return _response.BadRequest("User is not verified");
                var code = _utilities.GenerateCode();
                await _redis.Add(email, code, TimeSpan.FromMinutes(5));
                var resetEndpoint = $"{_config["URL"]}/resetpassword/?email={email}&code={code}";
                var subject = "Resetting Your Password";
                var body = $"Reset your Password by clicking this link : <a href='{resetEndpoint}'>Click Here</a>";
                await SendEmail(email,subject, body);
                return _response.Ok($"The Code is Sent Sucessfully to {_utilities.ShortenEmail(email)}");
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<ResponseStatus> ResetPassword(string email, string code, string newPassword)
        {
            try
            {
                if (!await _redis.ContainsKey(email)) return _response.BadRequest("Invalid Email");
                var user = await _db.Users.SingleOrDefaultAsync(y => y.Email == email);
                if (user == null) return _response.BadRequest("User is not exist");
                var storedCode = await _redis.Get(email);
                if(code != storedCode) return _response.BadRequest("Inavalid Code");
                await _redis.Delete(email);
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.Password = hashedPassword;
                await _db.SaveChangesAsync();
                return _response.Ok("The Password is Reset Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseStatus> Logout(int Id)
        {
            try
            {
                var user = await _db.Users.FindAsync(Id);
                if (user == null) return _response.BadRequest("User is not exist");
                if (await _redis.Get($"{user.Email}_refreshToken") == null) return _response.UnAuthorized("You are not Authorized");
                await _redis.Delete($"{user.Email}_refreshToken");
                return _response.Ok("Logged Out Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
