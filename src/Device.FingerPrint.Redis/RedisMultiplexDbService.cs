using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace Device.FingerPrint.Redis
{

    public interface IRedisMultiplexDbService
    {
        IDatabase GetDatabase(int db = -1);  // Access Redis database with an optional index
        IServer GetServer(string host, int port);  // Access Redis server for administrative commands
        IConnectionMultiplexer GetConnection();  // Direct access to the Redis connection

        Task HashSetMultipleFieldsWithDbIndexAsync(int dbIndex, string hashedKey, Dictionary<string, string> keyValuePairs, long expireInSecond);

        void HashSetMultipleFieldsWithDbIndexAsync(int dbIndex, string hashedKey, HashEntry[] hashEntries, long expireInSecond);

        Task<double> GetTimeSpan(int dbIndex, string hashedKey);

        List<string?> HashGetWithDbIndex(int dbIndex, string hashKey, List<string> fieldKeys);
    }
    public class RedisMultiplexDbService : IRedisMultiplexDbService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        // Constructor accepts configuration, such as Redis connection strings
        public RedisMultiplexDbService(IConfiguration configuration)
        {
            var redisConfiguration = configuration.GetSection("Redis").Get<RedisConfiguration>().ConfigurationOptions;
            
            _connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);
        }

        public IConnectionMultiplexer GetConnection()
        {
            return _connectionMultiplexer;
        }

        public IDatabase GetDatabase(int db = -1)
        {
            return _connectionMultiplexer.GetDatabase(db);
        }

        public IServer GetServer(string host, int port)
        {
            return _connectionMultiplexer.GetServer(host, port);
        }

        public async Task HashSetMultipleFieldsWithDbIndexAsync(int dbIndex, string hashedKey, Dictionary<string, string> keyValuePairs, long expireInSecond)
        {
            var db = this.GetDatabase(dbIndex);

            foreach (var pair in keyValuePairs)
            {
                await db.HashSetAsync(hashedKey, pair.Key, pair.Value);
            }

            await db.KeyExpireAsync(hashedKey, TimeSpan.FromSeconds(expireInSecond));
        }

        public void HashSetMultipleFieldsWithDbIndexAsync(int dbIndex, string hashedKey, HashEntry[] hashEntries, long expireInSecond)
        {
            var db = this.GetDatabase(dbIndex);

            db.HashSetAsync(hashedKey, hashEntries);

            db.KeyExpireAsync(hashedKey, TimeSpan.FromSeconds(expireInSecond));
        }

        public async Task<double> GetTimeSpan(int dbIndex, string hashedKey)
        {
            var db = this.GetDatabase(dbIndex);

            var timeToLive = await db.KeyTimeToLiveAsync(hashedKey);

            if (timeToLive != null && timeToLive.HasValue)
            {
                return timeToLive.Value.TotalSeconds;
            }

            return -1;
        }


        public List<string?> HashGetWithDbIndex(int dbIndex, string hashKey, List<string> fieldKeys)
        {
            var db = this.GetDatabase(dbIndex);

            RedisValue[] fields = fieldKeys.Select(f => (RedisValue)f).ToArray();
            RedisValue[] values = db.HashGet(hashKey, fields);

            return values.Select(value => value.HasValue ? value.ToString() : null).ToList();
        }
    }
}
