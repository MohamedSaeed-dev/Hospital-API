using HospitalAPI.Features.Redis.Service;
using Microsoft.CodeAnalysis.Differencing;
using StackExchange.Redis;

namespace HospitalAPI.Features.Redis.Repository
{
    public class RedisRepository : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        public RedisRepository(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }
        public async Task Add(string key, string value, TimeSpan time)
        {
            await _database.StringSetAsync(key, value, time);
        }

        public async Task<bool> ContainsKey(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task Delete(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<string?> Get(string key)
        {
            return await _database.StringGetAsync(key);
        }
    }
}
