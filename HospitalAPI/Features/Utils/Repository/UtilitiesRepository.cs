using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DataModels;
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

        public UtilitiesRepository(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateCode()
        {
            var code = "";
            for (int i = 0; i < 16; i++)
            {
                code += Convert.ToChar(65 + Random.Shared.Next(0, 26));
            }
            return code;
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
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
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
            var length = email.Length;
            var subEmail = email.Substring(0, 3);
            var newEmail = subEmail.PadRight(length, '*');
            return newEmail;
        }
    }
}
