using HospitalAPI.Features.Cookies.IServices;

namespace HospitalAPI.Features.Cookies.Repository
{
    public class CookieRepository : ICookieService
    {
        private readonly IHttpContextAccessor _http;
        public CookieRepository(IHttpContextAccessor http)
        {
            _http = http;
        }

        public void DeleteCookie(string key)
        {
            if (GetCookie(key) != null)
            {
                _http.HttpContext!.Response.Cookies.Delete(key);
            }
        }

        public string? GetCookie(string key)
        {
            string? cookie = _http.HttpContext?.Request.Cookies[key];
            return cookie;
        }

        public void SetCookie(string key, string value)
        {
            CookieOptions cookieOptions = new();
            cookieOptions.HttpOnly = true;
            cookieOptions.MaxAge = TimeSpan.FromDays(7);
            _http.HttpContext!.Response.Cookies.Append(key, value, cookieOptions);
        }
    }
}
