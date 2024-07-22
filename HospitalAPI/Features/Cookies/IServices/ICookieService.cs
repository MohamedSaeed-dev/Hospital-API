namespace HospitalAPI.Features.Cookies.IServices
{
    public interface ICookieService
    {
        string? GetCookie(string key);
        void SetCookie(string key, string value);
        void DeleteCookie(string key);
    }
}
