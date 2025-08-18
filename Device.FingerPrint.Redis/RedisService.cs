using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Device.FingerPrint.Redis
{
    public interface IRedisService
    {
        IRedisDatabase RedisDatabase { get; }

        IRedisDatabase RedisJSChallengeDatabase { get; }

        Task<T?> HashGetAsync<T>(string hashKey, string key);

        Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool isForceUpdate);

        Task<bool> HashExistAsync<T>(string hashKey, string key);

        Task<bool> UpdateExpiryAsync(string key, DateTimeOffset expiresAt);

        Task<long> HashDeleteAsync(string hashKey, string[] keys);

        Task HashSetAndExpiryAsync<T>(string hashKey, string key, T value, DateTimeOffset expiresAt, bool isForceUpdate = false);

        Task<T> GetAsync<T>(string key);

        Task<bool> SetAsync<T>(string key, T value);

        Task<bool> AddAsyncAndExpiryAsync<T>(string key, T value, DateTimeOffset expiresAt);

        Task<long> HashIncrementAsync(string hashKey, string key, long value);
    }

    public class RedisService : IRedisService
    {
        private readonly IRedisDatabase redisDatabase;
        private readonly IRedisDatabase redisJSChallengeDatabase;

        private readonly int REDIS_JSCHALLENGE_DATABASE = 10;

        public RedisService(IRedisClient redisClient)
        {
            redisDatabase = redisClient.GetDefaultDatabase();

            redisJSChallengeDatabase = redisClient.GetDb(REDIS_JSCHALLENGE_DATABASE);
        }

        public IRedisDatabase RedisDatabase => redisDatabase;

        public IRedisDatabase RedisJSChallengeDatabase => redisJSChallengeDatabase;

        public Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool isForceUpdate)
        {
            return redisDatabase.HashSetAsync<T>(hashKey, key, value, nx: !isForceUpdate, flag: CommandFlags.FireAndForget);
        }

        public async Task<T?> HashGetAsync<T>(string hashKey, string key)
        {
            return await redisDatabase.HashGetAsync<T>(hashKey ?? string.Empty, key);
        }

        public Task<bool> HashExistAsync<T>(string hashKey, string key)
        {
            return redisDatabase.HashExistsAsync(hashKey ?? string.Empty, key);
        }

        public Task<bool> UpdateExpiryAsync(string key, DateTimeOffset expiresAt)
        {
            return redisDatabase.UpdateExpiryAsync(key ?? string.Empty, expiresAt, CommandFlags.FireAndForget);
        }

        public Task<long> HashDeleteAsync(string hashKey, string[] keys)
        {
            return redisDatabase.HashDeleteAsync(hashKey ?? string.Empty, keys);
        }

        public async Task HashSetAndExpiryAsync<T>(string hashKey, string key, T value, DateTimeOffset expiresAt, bool isForceUpdate = false)
        {
            await HashSetAsync(hashKey, key, value, isForceUpdate);

            await UpdateExpiryAsync(hashKey, expiresAt);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await redisDatabase.GetAsync<T>(key);
        }

        public Task<bool> SetAsync<T>(string key, T value)
        {
            return redisDatabase.SetAddAsync<T>(key, value, flag: CommandFlags.FireAndForget);
        }

        public Task<bool> AddAsyncAndExpiryAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return redisDatabase.AddAsync(key, value, expiresAt, flag: CommandFlags.FireAndForget);
        }

        public Task<long> HashIncrementAsync(string hashKey, string key, long value)
        {
            return redisDatabase.HashIncrementByAsync(hashKey, key, value);
        }
    }
}
