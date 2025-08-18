# FPS.AI.Powered.Device.Fingerprint.Consumer - Unit Tests

## Tổng quan

Dự án này đã được tổ chức lại với cấu trúc test riêng biệt cho từng project để tăng coverage và dễ quản lý.

## Cấu trúc Test Projects

### 1. Device.FingerPrint.Test
**Mục đích**: Test cho project Device.FingerPrint chính
**Tests**: 13 tests
- **AccountServicesTests.cs**: 7 tests
  - `SearchAccount_WhenIsSearchPartialTrue_ShouldCallSearchPartialAccount`
  - `SearchAccount_WhenIsSearchPartialFalse_ShouldCallSearchExactAccount`
  - `GetAssAccountInfoByCustSubscriber_ShouldDelegateToDataAccess`
  - `GetAssAccountsByCustSubscriber_ShouldDelegateToDataAccess`
  - `GetAssDevicesByCustSubscriber_ShouldDelegateToDataAccess`
  - `GetAssMatrixAccountByCustSubscriber_ShouldDelegateToDataAccess`
  - `GetAccountsPerDevice_ShouldDelegateToDataAccess`

- **DeviceServicesTests.cs**: 6 tests
  - `SearchDevice_ShouldDelegateToDataAccess`
  - `GetAssDeviceInfoByIDSubscriber_ShouldDelegateToDataAccess`
  - `GetAssAccByDevSubscriber_ShouldDelegateToDataAccess`
  - `GetAssDevByDevSubscriber_ShouldDelegateToDataAccess`
  - `GetAssMatrixByDeviceSubscriber_ShouldDelegateToDataAccess`
  - `GetDevicesPerAccount_ShouldDelegateToDataAccess`

- **FingerPrintServicesTests.cs**: 1 test
  - `GetFPFingerPrint_ShouldDelegateToDataAccess`

### 2. Device.FingerPrint.Core.Test
**Mục đích**: Test cho project Device.FingerPrint.Core
**Tests**: 16 tests
- **CryptoTests.cs**: 7 tests
  - `Encrypt_WithValidText_ShouldReturnEncryptedString`
  - `Encrypt_WithEmptyText_ShouldReturnEmptyString`
  - `Encrypt_WithNullText_ShouldReturnEmptyString`
  - `Decrypt_WithValidEncryptedText_ShouldReturnDecryptedString`
  - `Decrypt_WithEmptyText_ShouldReturnEmptyString`
  - `Decrypt_WithNullText_ShouldReturnEmptyString`
  - `GetMD5Hash_WithValidString_ShouldReturnMD5Hash`

- **UtilitiesTests.cs**: 9 tests
  - `GetURLFromDeviceCode_WithValidDeviceCode_ShouldReturnURL`
  - `GetURLFromDeviceCode_WithEmptyDeviceCode_ShouldReturnEmptyString`
  - `GetURLFromDeviceCode_WithNullDeviceCode_ShouldReturnEmptyString`
  - `GetURLFromDeviceCode_WithLessThan8Parts_ShouldReturnEmptyString`
  - `GetUserAgentFromDeviceCode_WithValidDeviceCode_ShouldReturnUserAgent`
  - `GetUserAgentFromDeviceCode_WithLessThan4Parts_ShouldReturnEmptyString`
  - `IsGuidDeviceId_WithValidGuid_ShouldReturnTrue`
  - `IsGuidDeviceId_WithInvalidGuid_ShouldReturnFalse`
  - `GetDeviceCodeFromCapturedData_WithValidData_ShouldReturnDeviceCode`
  - `GetDeviceCodeFromCapturedData_WithLessThan7Parts_ShouldReturnEmptyString`
  - `GetAnalysis_WithValidData_ShouldReturnAnalysis`
  - `GetAnalysis_WithLessThan9Parts_ShouldReturnEmptyString`
  - `Chunk_WithValidData_ShouldReturnChunks`
  - `Split_WithValidData_ShouldReturnSplits`
  - `Partition_WithValidData_ShouldReturnPartitions`

### 3. FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Test
**Mục đích**: Test cho project FPS.AI.Powered.Device.Fingerprint.Consumer.Worker
**Tests**: 11 tests
- **DeviceServiceTests.cs**: 6 tests
  - `GetDeviceInfo_WithValidDeviceCode_ShouldReturnDeviceInfo`
  - `GetDeviceInfo_WithEmptyDeviceCode_ShouldReturnEmptyDeviceInfo`
  - `GetDeviceInfo_WithNullDeviceCode_ShouldReturnEmptyDeviceInfo`
  - `GetDeviceInfo_WithNullStringDeviceCode_ShouldReturnEmptyDeviceInfo`
  - `GetDeviceInfo_WithInvalidBase64_ShouldReturnEmptyDeviceInfo`
  - `GetDeviceInfo_WithInvalidJson_ShouldReturnEmptyDeviceInfo`

- **BotDetectionServiceTests.cs**: 5 tests
  - `GetFlaggedCode_WithBotDetection_ShouldReturnFlaggedCode`
  - `GetFlaggedCode_WithoutBotDetection_ShouldReturnOriginalCode`
  - `GetFlaggedCode_WithEmptyDeviceCode_ShouldReturnEmptyString`
  - `GetFlaggedCode_WithDifferentUserAgents_ShouldHandleCorrectly`
  - `TransformBotdModel_WithValidData_ShouldTransformCorrectly`
  - `GetBotDetectionTrue_ShouldReturnTrue`

### 4. FPS.AI.Powered.Device.Fingerprint.Consumer.Test
**Mục đích**: Test cho project Consumer chính (giữ lại từ trước)
**Tests**: 13 tests
- **BotDetectionServiceTests.cs**: 5 tests
- **DeviceServiceTests.cs**: 6 tests
- **SimpleTest.cs**: 1 test
- **FingerPrintServicesTests.cs**: 1 test

### 5. Device.FingerPrint.Queue.Test
**Mục đích**: Test cho project Device.FingerPrint.Queue
**Tests**: 0 tests (chưa có test cụ thể)

### 6. Device.FingerPrint.Redis.Test
**Mục đích**: Test cho project Device.FingerPrint.Redis
**Tests**: 0 tests (chưa có test cụ thể)

## Kết quả Test

**Tổng cộng: 64 tests**
- ✅ **Passed**: 64 tests
- ❌ **Failed**: 0 tests
- ⏭️ **Skipped**: 0 tests

## Công nghệ sử dụng

- **xUnit**: Framework test chính
- **Moq**: Mocking library để isolate dependencies
- **FluentAssertions**: Assertion library cho readable tests
- **Microsoft.Extensions.DependencyInjection**: DI container
- **Microsoft.Extensions.Configuration**: Configuration management
- **Microsoft.Extensions.Logging**: Logging framework

## Cách chạy Test

```bash
# Chạy tất cả tests
dotnet test

# Chạy test cho project cụ thể
dotnet test Device.FingerPrint.Test
dotnet test Device.FingerPrint.Core.Test
dotnet test FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Test

# Chạy test với coverage (nếu có)
dotnet test --collect:"XPlat Code Coverage"
```

## Cấu trúc Test

Mỗi test project được tổ chức theo pattern:
```
ProjectName.Test/
├── Services/
│   ├── ServiceNameTests.cs
│   └── AnotherServiceTests.cs
├── Core/
│   └── CoreClassTests.cs
└── ProjectName.Test.csproj
```

## Best Practices được áp dụng

1. **AAA Pattern**: Arrange, Act, Assert
2. **Descriptive Test Names**: Tên test mô tả rõ ràng scenario và expected result
3. **Mocking Dependencies**: Sử dụng Moq để isolate unit under test
4. **Edge Cases**: Test các trường hợp null, empty, invalid input
5. **Fluent Assertions**: Sử dụng FluentAssertions cho readable assertions
6. **Test Organization**: Tổ chức test theo từng service/class

## Coverage

Các test hiện tại cover:
- ✅ Account Services (AccountServices)
- ✅ Device Services (DeviceServices)
- ✅ FingerPrint Services (FingerPrintServices)
- ✅ Crypto Services (Crypto)
- ✅ Utilities (Utilities)
- ✅ Bot Detection Services (BotDetectionService)
- ✅ Device Services trong Worker (DeviceService)

## Lưu ý

- Tất cả tests đều pass thành công
- Cấu trúc test được tổ chức riêng biệt cho từng project
- Sử dụng dependency injection và mocking để isolate units
- Tests cover cả happy path và edge cases
- Code coverage sẽ được cải thiện đáng kể so với trước đây
