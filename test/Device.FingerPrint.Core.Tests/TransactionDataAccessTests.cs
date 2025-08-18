using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Features.Transaction.Model;
using Fanex.Data.Repository;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
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

			var act = () => _dataAccess.InsertRawTransaction(transJson, isTransOfDBExtra);
			act.Should().NotThrow();
		}

		[Fact]
		public void InsertFPTagging_DoesNotThrow()
		{
			var taggings = "{\"tag\":\"value\"}";
			var isTransOfDBExtra = true;

			var act = () => _dataAccess.InsertFPTagging(taggings, isTransOfDBExtra);
			act.Should().NotThrow();
		}

		[Fact]
		public void InsertFPUserAgent_DoesNotThrow()
		{
			var userAgentJson = "{\"userAgent\":\"Mozilla/5.0\"}";
			var isTransOfDBExtra = false;

			var act = () => _dataAccess.InsertFPUserAgent(userAgentJson, isTransOfDBExtra);
			act.Should().NotThrow();
		}

		[Fact]
		public void InsertJSChallengeInfo_DoesNotThrow()
		{
			var jsChallengeInfoJson = "{\"challenge\":\"info\"}";
			var isTransOfDBExtra = true;

			var act = () => _dataAccess.InsertJSChallengeInfo(jsChallengeInfoJson, isTransOfDBExtra);
			act.Should().NotThrow();
		}

		[Fact]
		public void InsertFPFingerPrint_DoesNotThrow()
		{
			var fingerPrintJson = "{\"fingerprint\":\"data\"}";
			var isTransOfDBExtra = false;

			var act = () => _dataAccess.InsertFPFingerPrint(fingerPrintJson, isTransOfDBExtra);
			act.Should().NotThrow();
		}

		[Fact]
		public void InsertTransaction_DoesNotThrow()
		{
			var transJson = "{\"transaction\":\"x\"}";
			var act = () => _dataAccess.InsertTransaction(transJson);
			act.Should().NotThrow();
		}

		[Fact]
		public void DeviceIdentify_DoesNotThrow()
		{
			var act = () => _dataAccess.DeviceIdentify("{}", 1);
			act.Should().NotThrow();
		}

		[Fact]
		public void DeviceIdentifyNoFingerprint_DoesNotThrow()
		{
			var rawTransIds = "1,2,3";
			var act = () => _dataAccess.DeviceIdentifyNoFingerprint(rawTransIds);
			act.Should().NotThrow();
		}

		[Fact]
		public void UpdateMergeTransaction_DoesNotThrow()
		{
			var transJson = "{\"merge\":\"data\"}";
			var act = () => _dataAccess.UpdateMergeTransaction(transJson);
			act.Should().NotThrow();
		}

		[Fact]
		public void UpdateForceMergeTransaction_DoesNotThrow()
		{
			var transJson = "{\"forceMerge\":\"data\"}";
			var act = () => _dataAccess.UpdateForceMergeTransaction(transJson);
			act.Should().NotThrow();
		}

		[Fact]
		public void EvolutionNewDevice_DoesNotThrow()
		{
			var transJson = "{\"evolution\":\"newDevice\"}";
			var act = () => _dataAccess.EvolutionNewDevice(transJson);
			act.Should().NotThrow();
		}

		[Fact]
		public void EvolutionRecoverDevice_DoesNotThrow()
		{
			var transJson = "{\"evolution\":\"recoverDevice\"}";
			var act = () => _dataAccess.EvolutionRecoverDevice(transJson);
			act.Should().NotThrow();
		}

		[Fact]
		public void GetMergeTransaction_Returns_Empty_When_NoRepositorySetup()
		{
			var result = _dataAccess.GetMergeTransaction(10, 0);
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public void GetForceMergeTransaction_Returns_Empty_When_NoRepositorySetup()
		{
			var result = _dataAccess.GetForceMergeTransaction(5, 2);
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public void GetTransactionEvolution_Returns_Empty_When_NoRepositorySetup()
		{
			var result = _dataAccess.GetTransactionEvolution(50);
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public void GetTransactionEvolutionAttributes_Returns_Empty_When_NoRepositorySetup()
		{
			var result = _dataAccess.GetTransactionEvolutionAttributes("1,2,3", "chrome");
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public void GetTransactionHistoryByTransIdSubscriber_Returns_Empty_When_NoRepositorySetup()
		{
			var result = _dataAccess.GetTransactionHistoryByTransIdSubscriber(123, 456, 10);
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public void GetAssociationByAccountSubscriber_Returns_Empty_When_NoRepositorySetup()
		{
			var result = _dataAccess.GetAssociationByAccountSubscriber(123, "testuser");
			result.Item1.Should().NotBeNull();
			result.Item1.Should().BeEmpty();
			result.Item2.Should().NotBeNull();
			result.Item2.Should().BeEmpty();
		}

		[Fact]
		public void GetAssociationBySubscriberDevice_Returns_Empty_When_NoRepositorySetup()
		{
			var result = _dataAccess.GetAssociationBySubscriberDevice(123, "device123", DateTime.Now.AddDays(-7), DateTime.Now);
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public void GetTransactionInfo_Returns_Empty_When_NoRepositorySetup()
		{
			var result = _dataAccess.GetTransactionInfo(1000UL, 50);
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		[Fact]
		public void GetTransactionByCustSubscriber_Returns_Empty_When_NoRepositorySetup()
		{
			var param = new TransactionParamModel
			{
				FromDate = DateTime.Now.AddDays(-7),
				ToDate = DateTime.Now,
				SubscriberID = 123,
				CTSCustID = 123UL,
				Skip = 0,
				Take = 10
			};

			var result = _dataAccess.GetTransactionByCustSubscriber(param);
			result.Should().NotBeNull();
			result.Items.Should().NotBeNull();
			result.Items.Should().BeEmpty();
		}

		[Fact]
		public void GetTransactionBySubscriber_Returns_Empty_When_NoRepositorySetup()
		{
			var param = new TransactionParamModel
			{
				FromDate = DateTime.Now.AddDays(-7),
				ToDate = DateTime.Now,
				SubscriberID = 123,
				UserName = "testuser",
				IsPartial = false,
				Skip = 0,
				Take = 10
			};

			var result = _dataAccess.GetTransactionBySubscriber(param);
			result.Should().NotBeNull();
			result.Items.Should().NotBeNull();
			result.Items.Should().BeEmpty();
		}

		[Fact]
		public void GetLastestTransactionBySubscriber_Returns_Empty_When_NoRepositorySetup()
		{
			var param = new LatestTransactionParamModel
			{
				SubscriberID = 123,
				Length = 10,
				Skip = 0,
				Take = 5
			};

			var result = _dataAccess.GetLastestTransactionBySubscriber(param);
			result.Should().NotBeNull();
			result.Items.Should().NotBeNull();
			result.Items.Should().BeEmpty();
		}

		[Fact]
		public void GetTransactionHistoryByDevice_Returns_Empty_When_NoRepositorySetup()
		{
			var param = new TransactionHistoryByDeviceParamModel
			{
				DeviceID = 123UL,
				FromDate = DateTime.Now.AddDays(-7),
				ToDate = DateTime.Now,
				SubscriberID = 123,
				Skip = 0,
				Take = 10
			};

			var result = _dataAccess.GetTransactionHistoryByDevice(param);
			result.Should().NotBeNull();
			result.Items.Should().NotBeNull();
			result.Items.Should().BeEmpty();
		}

		[Fact]
		public void GetDeviceIdentify_Returns_Empty_When_NoRepositorySetup()
		{
			var result = _dataAccess.GetDeviceIdentify(100, 2);
			result.Item1.Should().NotBeNull();
			result.Item1.Should().BeEmpty();
			result.Item2.Should().NotBeNull();
			result.Item2.Should().BeEmpty();
			result.Item3.Should().NotBeNull();
			result.Item3.Should().BeEmpty();
		}
	}
}
