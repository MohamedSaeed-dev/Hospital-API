namespace HospitalAPI.Features.Redis.Service
{
    public interface IRedisService
    {
        Task<string?> Get(string key);
        Task Delete(string key);
        Task Add(string key, string value, TimeSpan time);
        Task<bool> ContainsKey(string key);
    }
}
