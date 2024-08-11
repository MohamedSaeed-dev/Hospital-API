using AutoMapper;
using HospitalAPI.Features.Redis.Service;
using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.ViewModels;
using HospitalAPI.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalAPI.Features.Utils.Repository
{
    public class TokenRepository : ITokenService
    {
        private readonly IUtilitiesService _utilities;
        private readonly IResponseStatus _response;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public TokenRepository(IUtilitiesService utilities, IResponseStatus response, IConfiguration config, IMapper mapper)
        {
            _utilities = utilities;
            _response = response;
            _config = config;
            _mapper = mapper;
        }
        public ResponseStatus RefreshToken(string refreshToken)
        {
            var userViewModel = VerifyToken(refreshToken, "KeyRefreshToken");
            if(userViewModel == null) return _response.Forbidden("Forbidden");
            var user = _mapper.Map<User>(userViewModel);
            var accessToken = GenerateToken(user, "KeyAccessToken", DateTime.Now.AddMinutes(1));
            return _response.Ok(accessToken);
        }

        public UserViewModel? VerifyToken(string token, string key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(_config[$"Jwt:{key}"]!);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken verifiedToken);

                var jwtToken = (JwtSecurityToken)verifiedToken;
                var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
                var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;
                var role = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;
                if (Enum.TryParse(role, true, out Role result))
                {
                    return new UserViewModel
                    {
                        UserName = username,
                        Email = email,
                        Role = result
                    };
                }
                return null;
            }
            catch
            {
                return null;
            }
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
    }
}
