using StackExchange.Redis;

namespace Device.FingerPrint.Redis
{
    public interface IRedisMultiplexService
    {
        Task HashSetMultipleFieldsWithDbIndexAsync(int redisDatabaseIndex, string hashKey, Dictionary<string, string> fieldKeyValuePairs, long expireSecond);

        Task<string> HashGetWithDbIndexAsync(int redisDatabaseIndex, string hashKey, string fieldKey);

        Task<double> GetTimeSpan(int redisDatabaseIndex, string hashKey);

        IDatabase GetRedisEverTaggingDb();

        Task EverTagging_AddItemWithExpiryAsync(string hashKey, string itemKey, string value, TimeSpan expiry);

        Task<string> EverTagging_GetValueFromRedisAsync(string value);

        Task<string> EverTagging_GetValuesFromRedis(IEnumerable<string> values);

        Task<bool> EverTagging_DeleteValueFromRedis(string key);
    }

    public class RedisMultiplexService : IRedisMultiplexService
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;

        private readonly IDatabase redisEverTaggingDb;

        public RedisMultiplexService(IConnectionMultiplexer connectionMultiplexer, int EverTaggingDbIndex)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            redisEverTaggingDb = connectionMultiplexer.GetDatabase(EverTaggingDbIndex);
        }

        public IDatabase GetRedisEverTaggingDb()
        {
            return redisEverTaggingDb;
        }

        public async Task HashSetMultipleFieldsWithDbIndexAsync(int redisDatabaseIndex, string hashKey, Dictionary<string, string> fieldKeyValuePairs, long expireSecond)
        {
            var db = connectionMultiplexer.GetDatabase(redisDatabaseIndex);

            foreach (var pair in fieldKeyValuePairs)
            {
                await db.HashSetAsync(hashKey, pair.Key, pair.Value);
            }

            await db.KeyExpireAsync(hashKey, TimeSpan.FromSeconds(expireSecond));
        }

        public async Task<string> HashGetWithDbIndexAsync(int redisDatabaseIndex, string hashKey, string fieldKey)
        {
            var db = connectionMultiplexer.GetDatabase(redisDatabaseIndex);

            return await db.HashGetAsync(hashKey, fieldKey);
        }

        public async Task<double> GetTimeSpan(int redisDatabaseIndex, string hashKey)
        {
            var db = connectionMultiplexer.GetDatabase(redisDatabaseIndex);
            var timeToLive = await db.KeyTimeToLiveAsync(hashKey);

            if (timeToLive != null && timeToLive.HasValue)
            {
                return timeToLive.Value.TotalSeconds;
            }

            return -1;
        }

        public async Task EverTagging_AddItemWithExpiryAsync(string hashKey, string itemKey, string value, TimeSpan expiry)
        {
            string expiryKey = $"{hashKey}:{itemKey}";
            await redisEverTaggingDb.StringSetAsync(expiryKey, value);
            await redisEverTaggingDb.KeyExpireAsync(expiryKey, expiry);
        }

        public async Task<string> EverTagging_GetValueFromRedisAsync(string value)
        {
            var result = await redisEverTaggingDb.StringGetAsync(value);
            if (result.HasValue)
            {
                return result.ToString();
            }

            return string.Empty;
        }

        public async Task<string> EverTagging_GetValuesFromRedis(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                var result = await redisEverTaggingDb.StringGetAsync(value);
                if (result.HasValue)
                {
                    return value;
                }
            }
            return string.Empty;
        }

        public async Task<bool> EverTagging_DeleteValueFromRedis(string key)
        {
            return await redisEverTaggingDb.KeyDeleteAsync(key);
        }
    }
}
