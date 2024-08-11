using AutoMapper;
using HospitalAPI.Features;
using HospitalAPI.Features.Mail.Service;
using HospitalAPI.Features.Redis.Service;
using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Models.ViewModels;
using HospitalAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
                string refreshToken = _token.GenerateToken(record, "KeyRefreshToken", DateTime.Now.AddDays(7));
                string accessToken = _token.GenerateToken(record, "KeyAccessToken", DateTime.Now.AddMinutes(3));
                var login = new LoginViewModel
                {
                    Warning = !record.IsVerified ? "Your account is not verifed yet" : null,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
                return _response.Ok(login);
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
                var record = await _db.Users.SingleOrDefaultAsync(x => x.UserName == user.UserName || x.Email == user.Email);
                if (record != null) return _response.BadRequest("User is already exist");
                var jsonUser = JsonSerializer.Serialize(user);
                await _redis.Add($"{user.Email}_User", jsonUser, TimeSpan.FromMinutes(10));
                var response = await _utilities.SendOTP(user.Email);
                return _response.Custom(response.StatusCode, response.Message!);  
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ResponseStatus> VerifyEmail(string email, string otp)
        {
            try
            {
                var containsKey = await _redis.ContainsKey($"{email}_OTP");
                if (!containsKey) return _response.BadRequest("The email is Incorrect");
                var storedOTP = await _redis.Get($"{email}_OTP");
                if (otp != storedOTP) return _response.BadRequest("The OTP is Incorrect");
                await _redis.Delete($"{email}_OTP");
                var isContainUser = await _redis.ContainsKey($"{email}_User");
                if(!isContainUser) return _response.BadRequest("No such user");
                var jsonUser = await _redis.Get($"{email}_User");
                UserSignUp user = JsonSerializer.Deserialize<UserSignUp>(jsonUser!)!;
                await _redis.Delete($"{email}_User");
                User newUser = _mapper.Map<User>(user);
                var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                newUser.Password = hashPassword;
                newUser.Role = Role.Receptionist;
                newUser.IsVerified = true;
                await _db.Users.AddAsync(newUser);
                await _db.SaveChangesAsync();
                string refreshToken = _token.GenerateToken(newUser, "KeyRefreshToken", DateTime.Now.AddDays(7));
                string accessToken = _token.GenerateToken(newUser, "KeyAccessToken", DateTime.Now.AddMinutes(3));
                var signup = new SignupViewModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
                return _response.Ok(signup);
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
                await _redis.Add($"{email}_Code", code, TimeSpan.FromMinutes(5));
                var resetEndpoint = $"{_config["URL"]}/resetpassword/?email={email}&code={code}";
                var subject = "Resetting Your Password";
                var body = $"Reset your Password by clicking this link : <a href='{resetEndpoint}'>Click Here</a>";
                await _utilities.SendEmail(email,subject, body);
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
                var storedCode = await _redis.Get($"{email}_Code");
                if(code != storedCode) return _response.BadRequest("Inavalid Code");
                await _redis.Delete($"{email}_Code");
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
    }
}
