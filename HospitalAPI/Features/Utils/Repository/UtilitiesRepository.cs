using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.ViewModels.ResponseStatus;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace HospitalAPI.Features.Utils.Repository
{
    public class UtilitiesRepository : IUtilitiesService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _http;
        private readonly IResponseStatus _response;
        private const string chars = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";
        public UtilitiesRepository(IConfiguration config, IHttpContextAccessor http, IResponseStatus response)
        {
            _config = config;
            _http = http;
            _response = response;
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

        public string GenerateToken(User user, string key, DateTime time)
        {
            var keySecurity = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config[$"Jwt:{key}"]!));
            var credentials = new SigningCredentials(keySecurity, SecurityAlgorithms.HmacSha256);
            var role = Enum.GetName(typeof(Role), user.Role);
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role!)
            };
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: time,
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
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

        public bool VerifyToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:KeyAccessToken"]!);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken verifiedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /*public Task RefreshToken()
        {

        }*/
    }
}
