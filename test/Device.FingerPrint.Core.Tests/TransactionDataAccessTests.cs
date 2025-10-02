using Device.FingerPrint.Features.Transaction.Services;
using Fanex.Data.Repository;
using FluentAssertions;
using Moq;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class TransactionDataAccessTests
    {
        private readonly Mock<IDynamicRepository> _mockRepository;
        private readonly TransactionDataAccess _dataAccess;

        public TransactionDataAccessTests()
        {
            _mockRepository = new Mock<IDynamicRepository>(MockBehavior.Loose);
            _dataAccess = new TransactionDataAccess();

            var repositoryField = typeof(TransactionDataAccess).GetField("repository",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            repositoryField?.SetValue(_dataAccess, _mockRepository.Object);
        }

        [Fact]
        public void InsertRawTransaction_DoesNotThrow()
        {
            var transJson = "{\"test\":\"data\"}";
            var isTransOfDBExtra = false;

            var act = () => _dataAccess.InsertRawTransaction(transJson);
            act.Should().NotThrow();
        }
    }
}
