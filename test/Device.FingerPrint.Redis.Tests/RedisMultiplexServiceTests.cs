using FluentAssertions;
using Moq;
using StackExchange.Redis;

namespace Device.FingerPrint.Redis.Tests
{
    public class RedisMultiplexServiceTests
    {
        private readonly Mock<IConnectionMultiplexer> _mockMuxer;
        private readonly Mock<IDatabase> _mockDb;
        private readonly RedisMultiplexService _service;

        public RedisMultiplexServiceTests()
        {
            _mockMuxer = new Mock<IConnectionMultiplexer>(MockBehavior.Loose);
            _mockDb = new Mock<IDatabase>(MockBehavior.Strict);
            _mockMuxer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_mockDb.Object);

            _service = new RedisMultiplexService(_mockMuxer.Object, EverTaggingDbIndex: 1);
        }

        [Fact]
        public void GetRedisEverTaggingDb_ShouldReturnDb()
        {
            var db = _service.GetRedisEverTaggingDb();
            db.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_ShouldUseCorrectEverTaggingDbIndex()
        {
            _mockMuxer.Verify(x => x.GetDatabase(1, It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task HashSetMultipleFieldsWithDbIndexAsync_ShouldSetValuesAndExpire()
        {
            _mockDb.Setup(db => db.HashSetAsync("h", "k1", "v1", When.Always, CommandFlags.None)).ReturnsAsync(true);
            _mockDb.Setup(db => db.HashSetAsync("h", "k2", "v2", When.Always, CommandFlags.None)).ReturnsAsync(true);
            _mockDb.Setup(db => db.KeyExpireAsync("h", It.IsAny<TimeSpan?>(), It.IsAny<ExpireWhen>(), CommandFlags.None)).ReturnsAsync(true);

            await _service.HashSetMultipleFieldsWithDbIndexAsync(0, "h", new Dictionary<string, string> { { "k1", "v1" }, { "k2", "v2" } }, 60);
        }

        [Fact]
        public async Task HashSetMultipleFieldsWithDbIndexAsync_WithEmptyDictionary_ShouldOnlySetExpiry()
        {
            _mockDb.Setup(db => db.KeyExpireAsync("h", It.IsAny<TimeSpan?>(), It.IsAny<ExpireWhen>(), CommandFlags.None)).ReturnsAsync(true);

            await _service.HashSetMultipleFieldsWithDbIndexAsync(0, "h", new Dictionary<string, string>(), 60);
        }

        [Fact]
        public async Task HashSetMultipleFieldsWithDbIndexAsync_WithZeroExpire_ShouldNotSetExpiry()
        {
            _mockDb.Setup(db => db.HashSetAsync("h", "k1", "v1", When.Always, CommandFlags.None)).ReturnsAsync(true);
            _mockDb.Setup(db => db.KeyExpireAsync("h", TimeSpan.FromSeconds(0), It.IsAny<ExpireWhen>(), CommandFlags.None)).ReturnsAsync(true);

            await _service.HashSetMultipleFieldsWithDbIndexAsync(0, "h", new Dictionary<string, string> { { "k1", "v1" } }, 0);
        }

        [Fact]
        public async Task HashGetWithDbIndexAsync_ShouldReturnValue()
        {
            _mockDb.Setup(db => db.HashGetAsync("h", "k", CommandFlags.None)).ReturnsAsync("v");
            var v = await _service.HashGetWithDbIndexAsync(0, "h", "k");
            v.Should().Be("v");
        }

        [Fact]
        public async Task HashGetWithDbIndexAsync_ShouldReturnEmptyString()
        {
            _mockDb.Setup(db => db.HashGetAsync("h", "k", CommandFlags.None)).ReturnsAsync(RedisValue.Null);
            var v = await _service.HashGetWithDbIndexAsync(0, "h", "k");
            v.Should().BeNull();
        }

        [Fact]
        public async Task HashGetWithDbIndexAsync_WithDifferentDatabaseIndex_ShouldUseCorrectDb()
        {
            _mockDb.Setup(db => db.HashGetAsync("h", "k", CommandFlags.None)).ReturnsAsync("v");
            var v = await _service.HashGetWithDbIndexAsync(5, "h", "k");
            v.Should().Be("v");
            _mockMuxer.Verify(x => x.GetDatabase(5, It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task GetTimeSpan_ShouldReturnSeconds()
        {
            _mockDb.Setup(db => db.KeyTimeToLiveAsync("h", CommandFlags.None)).ReturnsAsync(TimeSpan.FromSeconds(42));
            var s = await _service.GetTimeSpan(0, "h");
            s.Should().Be(42);
        }

        [Fact]
        public async Task GetTimeSpan_ShouldReturnNegativeOne()
        {
            _mockDb.Setup(db => db.KeyTimeToLiveAsync("h", CommandFlags.None)).ReturnsAsync((TimeSpan?)null);
            var s = await _service.GetTimeSpan(0, "h");
            s.Should().Be(-1);
        }

        [Fact]
        public async Task GetTimeSpan_WithDifferentDatabaseIndex_ShouldUseCorrectDb()
        {
            _mockDb.Setup(db => db.KeyTimeToLiveAsync("h", CommandFlags.None)).ReturnsAsync(TimeSpan.FromSeconds(30));
            var s = await _service.GetTimeSpan(3, "h");
            s.Should().Be(30);
            _mockMuxer.Verify(x => x.GetDatabase(3, It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task EverTagging_AddItemWithExpiryAsync_ShouldSetValueAndExpiry()
        {
            _mockDb.Setup(db => db.StringSetAsync("hash:item", "val", null, false, When.Always, CommandFlags.None)).ReturnsAsync(true);
            _mockDb.Setup(db => db.KeyExpireAsync("hash:item", It.IsAny<TimeSpan?>(), It.IsAny<ExpireWhen>(), CommandFlags.None)).ReturnsAsync(true);
            await _service.EverTagging_AddItemWithExpiryAsync("hash", "item", "val", TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task EverTagging_AddItemWithExpiryAsync_WithEmptyValue_ShouldWork()
        {
            _mockDb.Setup(db => db.StringSetAsync("hash:item", "", null, false, When.Always, CommandFlags.None)).ReturnsAsync(true);
            _mockDb.Setup(db => db.KeyExpireAsync("hash:item", It.IsAny<TimeSpan?>(), It.IsAny<ExpireWhen>(), CommandFlags.None)).ReturnsAsync(true);
            await _service.EverTagging_AddItemWithExpiryAsync("hash", "item", "", TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task EverTagging_AddItemWithExpiryAsync_WithZeroExpiry_ShouldWork()
        {
            _mockDb.Setup(db => db.StringSetAsync("hash:item", "val", null, false, When.Always, CommandFlags.None)).ReturnsAsync(true);
            _mockDb.Setup(db => db.KeyExpireAsync("hash:item", It.IsAny<TimeSpan?>(), It.IsAny<ExpireWhen>(), CommandFlags.None)).ReturnsAsync(true);
            await _service.EverTagging_AddItemWithExpiryAsync("hash", "item", "val", TimeSpan.Zero);
        }

        [Fact]
        public async Task EverTagging_GetValueFromRedisAsync_ShouldReturnValue()
        {
            _mockDb.Setup(db => db.StringGetAsync("key", CommandFlags.None)).ReturnsAsync((RedisValue)"val");
            var got = await _service.EverTagging_GetValueFromRedisAsync("key");
            got.Should().Be("val");
        }

        [Fact]
        public async Task EverTagging_GetValueFromRedisAsync_ShouldReturnEmptyString()
        {
            _mockDb.Setup(db => db.StringGetAsync("key", CommandFlags.None)).ReturnsAsync(RedisValue.Null);
            var got = await _service.EverTagging_GetValueFromRedisAsync("key");
            got.Should().BeEmpty();
        }

        [Fact]
        public async Task EverTagging_GetValueFromRedisAsync_WithEmptyKey_ShouldWork()
        {
            _mockDb.Setup(db => db.StringGetAsync("", CommandFlags.None)).ReturnsAsync((RedisValue)"val");
            var got = await _service.EverTagging_GetValueFromRedisAsync("");
            got.Should().Be("val");
        }

        [Fact]
        public async Task EverTagging_GetValuesFromRedis_ShouldReturnFirstFoundValue()
        {
            _mockDb.Setup(db => db.StringGetAsync("a", CommandFlags.None)).ReturnsAsync(RedisValue.Null);
            _mockDb.Setup(db => db.StringGetAsync("b", CommandFlags.None)).ReturnsAsync((RedisValue)"b");
            var gotAny = await _service.EverTagging_GetValuesFromRedis(new[] { "a", "b" });
            gotAny.Should().Be("b");
        }

        [Fact]
        public async Task EverTagging_GetValuesFromRedis_ShouldReturnEmptyString()
        {
            _mockDb.Setup(db => db.StringGetAsync("a", CommandFlags.None)).ReturnsAsync(RedisValue.Null);
            _mockDb.Setup(db => db.StringGetAsync("b", CommandFlags.None)).ReturnsAsync(RedisValue.Null);
            var gotAny = await _service.EverTagging_GetValuesFromRedis(new[] { "a", "b" });
            gotAny.Should().BeEmpty();
        }

        [Fact]
        public async Task EverTagging_GetValuesFromRedis_WithEmptyArray_ShouldReturnEmptyString()
        {
            var gotAny = await _service.EverTagging_GetValuesFromRedis(new string[0]);
            gotAny.Should().BeEmpty();
        }

        [Fact]
        public async Task EverTagging_GetValuesFromRedis_WithSingleValue_ShouldWork()
        {
            _mockDb.Setup(db => db.StringGetAsync("a", CommandFlags.None)).ReturnsAsync((RedisValue)"a");
            var gotAny = await _service.EverTagging_GetValuesFromRedis(new[] { "a" });
            gotAny.Should().Be("a");
        }

        [Fact]
        public async Task EverTagging_DeleteValueFromRedis_ShouldReturnTrue()
        {
            _mockDb.Setup(db => db.KeyDeleteAsync("del", CommandFlags.None)).ReturnsAsync(true);
            var deleted = await _service.EverTagging_DeleteValueFromRedis("del");
            deleted.Should().BeTrue();
        }

        [Fact]
        public async Task EverTagging_DeleteValueFromRedis_ShouldReturnFalse()
        {
            _mockDb.Setup(db => db.KeyDeleteAsync("del", CommandFlags.None)).ReturnsAsync(false);
            var deleted = await _service.EverTagging_DeleteValueFromRedis("del");
            deleted.Should().BeFalse();
        }

        [Fact]
        public async Task EverTagging_DeleteValueFromRedis_WithEmptyKey_ShouldWork()
        {
            _mockDb.Setup(db => db.KeyDeleteAsync("", CommandFlags.None)).ReturnsAsync(true);
            var deleted = await _service.EverTagging_DeleteValueFromRedis("");
            deleted.Should().BeTrue();
        }

        [Fact]
        public void Ctor_WithDifferentEverTaggingDbIndex_ShouldUseCorrectIndex()
        {
            var service = new RedisMultiplexService(_mockMuxer.Object, EverTaggingDbIndex: 5);
            _mockMuxer.Verify(x => x.GetDatabase(5, It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void GetRedisEverTaggingDb_ShouldReturnSameInstance()
        {
            var db1 = _service.GetRedisEverTaggingDb();
            var db2 = _service.GetRedisEverTaggingDb();
            db1.Should().BeSameAs(db2);
        }
    }
}
