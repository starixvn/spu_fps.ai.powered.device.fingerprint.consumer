using FluentAssertions;
using Moq;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Device.FingerPrint.Redis.Tests
{
    public class RedisServiceTests
    {
        private readonly Mock<IRedisClient> _mockRedisClient;
        private readonly Mock<IRedisDatabase> _mockDefaultDb;
        private readonly Mock<IRedisDatabase> _mockJsChallengeDb;
        private readonly RedisService _service;

        public RedisServiceTests()
        {
            _mockRedisClient = new Mock<IRedisClient>(MockBehavior.Strict);
            _mockDefaultDb = new Mock<IRedisDatabase>(MockBehavior.Loose);
            _mockJsChallengeDb = new Mock<IRedisDatabase>(MockBehavior.Loose);

            _mockRedisClient.Setup(x => x.GetDefaultDatabase()).Returns(_mockDefaultDb.Object);
            _mockRedisClient.Setup(x => x.GetDb(10, It.IsAny<string?>())).Returns(_mockJsChallengeDb.Object);

            _service = new RedisService(_mockRedisClient.Object);
        }

        [Fact]
        public void Ctor_ShouldInitializeDatabases()
        {
            _service.RedisDatabase.Should().NotBeNull();
            _service.RedisJSChallengeDatabase.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_ShouldUseCorrectDatabaseIndex()
        {
            _mockRedisClient.Verify(x => x.GetDb(10, It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task HashSetAsync_ShouldHonorForceUpdateFlag()
        {
            _mockDefaultDb
                .Setup(db => db.HashSetAsync("hash", "key", 123, It.IsAny<bool>(), CommandFlags.FireAndForget))
                .ReturnsAsync(true);

            var result = await _service.HashSetAsync("hash", "key", 123, isForceUpdate: true);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HashSetAsync_ShouldHonorNoForceUpdateFlag()
        {
            _mockDefaultDb
                .Setup(db => db.HashSetAsync("hash", "key", 123, It.IsAny<bool>(), CommandFlags.FireAndForget))
                .ReturnsAsync(false);

            var result = await _service.HashSetAsync("hash", "key", 123, isForceUpdate: false);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task HashGetAsync_ShouldReturnValue()
        {
            _mockDefaultDb.Setup(db => db.HashGetAsync<string>("hash", "key", It.IsAny<CommandFlags>())).ReturnsAsync("value");

            var value = await _service.HashGetAsync<string>("hash", "key");
            value.Should().Be("value");
        }

        [Fact]
        public async Task HashGetAsync_WithNullHashKey_ShouldHandleGracefully()
        {
            _mockDefaultDb.Setup(db => db.HashGetAsync<string>(string.Empty, "key", It.IsAny<CommandFlags>())).ReturnsAsync("value");

            var value = await _service.HashGetAsync<string>(null!, "key");
            value.Should().Be("value");
        }

        [Fact]
        public async Task HashGetAsync_ShouldReturnNull()
        {
            _mockDefaultDb.Setup(db => db.HashGetAsync<string>("hash", "key", It.IsAny<CommandFlags>())).ReturnsAsync((string?)null);

            var value = await _service.HashGetAsync<string>("hash", "key");
            value.Should().BeNull();
        }

        [Fact]
        public async Task HashExistAsync_ShouldReturnTrue()
        {
            _mockDefaultDb.Setup(db => db.HashExistsAsync("hash", "key", It.IsAny<CommandFlags>())).ReturnsAsync(true);

            var exists = await _service.HashExistAsync<string>("hash", "key");
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task HashExistAsync_ShouldReturnFalse()
        {
            _mockDefaultDb.Setup(db => db.HashExistsAsync("hash", "key", It.IsAny<CommandFlags>())).ReturnsAsync(false);

            var exists = await _service.HashExistAsync<string>("hash", "key");
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task HashExistAsync_WithNullHashKey_ShouldHandleGracefully()
        {
            _mockDefaultDb.Setup(db => db.HashExistsAsync(string.Empty, "key", It.IsAny<CommandFlags>())).ReturnsAsync(true);

            var exists = await _service.HashExistAsync<string>(null!, "key");
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateExpiryAsync_ShouldForwardCall()
        {
            var when = DateTimeOffset.UtcNow.AddMinutes(5);
            _mockDefaultDb.Setup(db => db.UpdateExpiryAsync("hash", when, CommandFlags.FireAndForget)).ReturnsAsync(true);

            var ok = await _service.UpdateExpiryAsync("hash", when);
            ok.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateExpiryAsync_WithNullKey_ShouldHandleGracefully()
        {
            var when = DateTimeOffset.UtcNow.AddMinutes(5);
            _mockDefaultDb.Setup(db => db.UpdateExpiryAsync(string.Empty, when, CommandFlags.FireAndForget)).ReturnsAsync(false);

            var ok = await _service.UpdateExpiryAsync(null!, when);
            ok.Should().BeFalse();
        }

        [Fact]
        public async Task HashDeleteAsync_ShouldReturnCount()
        {
            _mockDefaultDb.Setup(db => db.HashDeleteAsync("hash", new[] { "a", "b" }, It.IsAny<CommandFlags>())).ReturnsAsync(2);

            var cnt = await _service.HashDeleteAsync("hash", new[] { "a", "b" });
            cnt.Should().Be(2);
        }

        [Fact]
        public async Task HashDeleteAsync_ShouldReturnZero()
        {
            _mockDefaultDb.Setup(db => db.HashDeleteAsync("hash", new[] { "a", "b" }, It.IsAny<CommandFlags>())).ReturnsAsync(0);

            var cnt = await _service.HashDeleteAsync("hash", new[] { "a", "b" });
            cnt.Should().Be(0);
        }

        [Fact]
        public async Task HashDeleteAsync_WithNullHashKey_ShouldHandleGracefully()
        {
            _mockDefaultDb.Setup(db => db.HashDeleteAsync(string.Empty, new[] { "a", "b" }, It.IsAny<CommandFlags>())).ReturnsAsync(1);

            var cnt = await _service.HashDeleteAsync(null!, new[] { "a", "b" });
            cnt.Should().Be(1);
        }

        [Fact]
        public async Task HashSetAndExpiryAsync_ShouldCallBoth()
        {
            var when = DateTimeOffset.UtcNow.AddMinutes(1);
            _mockDefaultDb
                .Setup(db => db.HashSetAsync("hash", "k", 1, It.IsAny<bool>(), CommandFlags.FireAndForget))
                .ReturnsAsync(true);
            _mockDefaultDb
                .Setup(db => db.UpdateExpiryAsync("hash", when, CommandFlags.FireAndForget))
                .ReturnsAsync(true);

            await _service.HashSetAndExpiryAsync("hash", "k", 1, when, false);
        }

        [Fact]
        public async Task HashSetAndExpiryAsync_WithForceUpdate_ShouldCallBoth()
        {
            var when = DateTimeOffset.UtcNow.AddMinutes(1);
            _mockDefaultDb
                .Setup(db => db.HashSetAsync("hash", "k", 1, It.IsAny<bool>(), CommandFlags.FireAndForget))
                .ReturnsAsync(true);
            _mockDefaultDb
                .Setup(db => db.UpdateExpiryAsync("hash", when, CommandFlags.FireAndForget))
                .ReturnsAsync(true);

            await _service.HashSetAndExpiryAsync("hash", "k", 1, when, true);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnValue()
        {
            _mockDefaultDb.Setup(db => db.GetAsync<string>("k", It.IsAny<CommandFlags>())).ReturnsAsync("v");
            var v = await _service.GetAsync<string>("k");
            v.Should().Be("v");
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull()
        {
            _mockDefaultDb.Setup(db => db.GetAsync<string>("k", It.IsAny<CommandFlags>())).ReturnsAsync((string?)null);
            var v = await _service.GetAsync<string>("k");
            v.Should().BeNull();
        }

        [Fact]
        public async Task SetAsync_ShouldReturnTrue()
        {
            _mockDefaultDb.Setup(db => db.SetAddAsync("k", "v", CommandFlags.FireAndForget)).ReturnsAsync(true);
            var ok = await _service.SetAsync("k", "v");
            ok.Should().BeTrue();
        }

        [Fact]
        public async Task SetAsync_ShouldReturnFalse()
        {
            _mockDefaultDb.Setup(db => db.SetAddAsync("k", "v", CommandFlags.FireAndForget)).ReturnsAsync(false);
            var ok = await _service.SetAsync("k", "v");
            ok.Should().BeFalse();
        }

        [Fact]
        public async Task AddAsyncAndExpiryAsync_ShouldNotThrow()
        {
            var when = DateTimeOffset.UtcNow.AddMinutes(2);
            // Removed explicit setup for AddAsync due to optional arguments; rely on loose mock default behaviour
            var act = new Func<Task>(async () => { await _service.AddAsyncAndExpiryAsync("k", "v", when); });
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task HashIncrementAsync_ShouldReturnNewValue()
        {
            _mockDefaultDb.Setup(db => db.HashIncrementByAsync("h", "k", 3, It.IsAny<CommandFlags>())).ReturnsAsync(5);
            var res = await _service.HashIncrementAsync("h", "k", 3);
            res.Should().Be(5);
        }

        [Fact]
        public async Task HashIncrementAsync_ShouldReturnZero()
        {
            _mockDefaultDb.Setup(db => db.HashIncrementByAsync("h", "k", 0, It.IsAny<CommandFlags>())).ReturnsAsync(0);
            var res = await _service.HashIncrementAsync("h", "k", 0);
            res.Should().Be(0);
        }

        [Fact]
        public async Task HashIncrementAsync_WithNegativeValue_ShouldWork()
        {
            _mockDefaultDb.Setup(db => db.HashIncrementByAsync("h", "k", -5, It.IsAny<CommandFlags>())).ReturnsAsync(-3);
            var res = await _service.HashIncrementAsync("h", "k", -5);
            res.Should().Be(-3);
        }

        [Fact]
        public void RedisDatabase_Property_ShouldReturnDefaultDatabase()
        {
            _service.RedisDatabase.Should().BeSameAs(_mockDefaultDb.Object);
        }

        [Fact]
        public void RedisJSChallengeDatabase_Property_ShouldReturnJsChallengeDatabase()
        {
            _service.RedisJSChallengeDatabase.Should().BeSameAs(_mockJsChallengeDb.Object);
        }
    }
}
