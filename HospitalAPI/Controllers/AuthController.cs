using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUtilitiesService _utilities;
        private readonly ITokenService _token;
        public AuthController(IAuthService authService, IUtilitiesService utilities, ITokenService token)
        {
            _authService = authService;
            _utilities = utilities;
            _token = token;
        }
        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp(UserSignUp user)
        {
            try
            {
                if (user == null) return BadRequest(new { message = "Invalid User data" });
                if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password)) return BadRequest(new { message = "Invalid User data" });
                if (!_utilities.IsEmail(user.Email)) return BadRequest(new { message = "Incorrect email" });
                var response = await _authService.SignUp(user);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin user)
        {
            try
            {
                if (user == null) return BadRequest(new { message = "Invalid User data" });
                if (string.IsNullOrEmpty( user.Email) || string.IsNullOrEmpty(user.Password)) return BadRequest(new { message = "Invalid User data" });
                var response = await _authService.Login(user);
                return StatusCode(response.StatusCode, new { response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }

        }
        [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP(string Email)
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || !_utilities.IsEmail(Email)) return BadRequest(new { message = "Invalid email" });
                var response = await _utilities.SendOTP(Email);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> Refresh([FromQuery] string Email)
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || !_utilities.IsEmail(Email)) return BadRequest(new { message = "Invalid email" });
                var response = await _token.RefreshToken(Email);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpPut("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string OTP, [FromQuery] string Email)
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(OTP) || !_utilities.IsEmail(Email)) return BadRequest(new { message = "Invalid data" });
                var response = await _authService.VerifyEmail(OTP, Email);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string Email)
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || !_utilities.IsEmail(Email)) return BadRequest(new { message = "Invalid email" });
                var response =  await _authService.ForgotPassword(Email);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromQuery]string Email, [FromQuery]string code, [FromBody]string newPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(code) || !_utilities.IsEmail(Email)) return BadRequest(new { message = "Invalid data" });
                var response = await _authService.ResetPassword(Email, code, newPassword);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpPost("Logout/{Id}")]
        public async Task<IActionResult> Logout(int Id)
        {
            try
            {
                if (Id <= 0) return BadRequest(new { message = "Invalid Id" });
                var response = await _authService.Logout(Id);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpGet("google/login")]
        public IActionResult GoogleAuth()
        {
            var props = new AuthenticationProperties { RedirectUri = "api/auth/signin-google" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("throw")]
        public IActionResult Throw()
        {
            throw new Exception("yes");
        }
        [HttpGet("google/signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (response.Principal == null) return BadRequest();
            var claims = response.Principal.Claims;
            var userInfo = new
            {
                AllClaims = claims.Select(c => new { c.Value })
            };
            return Ok(userInfo);
        }
    }
}