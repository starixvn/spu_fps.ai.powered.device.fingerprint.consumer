# Unit Tests cho FPS.AI.Powered.Device.Fingerprint.Consumer

## Tổng quan

Dự án này chứa các unit tests cho solution FPS.AI.Powered.Device.Fingerprint.Consumer, sử dụng xUnit framework với Moq cho mocking và FluentAssertions cho assertions.

## Cấu trúc Tests

### 1. SimpleTest.cs
- **Mục đích**: Test cơ bản để đảm bảo test framework hoạt động
- **Test cases**: 1 test đơn giản

### 2. Services/DeviceServiceTests.cs
- **Mục đích**: Test cho DeviceService
- **Test cases**: 5 tests
  - `GetDeviceInfo_WithValidDeviceCode_ShouldReturnDeviceInfo`
  - `GetDeviceInfo_WithEmptyDeviceCode_ShouldReturnDefaultDeviceInfo`
  - `GetDeviceInfo_WithNullDeviceCode_ShouldReturnDefaultDeviceInfo`
  - `GetDeviceInfo_WithInvalidBase64_ShouldReturnDefaultDeviceInfo`

### 3. Services/BotDetectionServiceTests.cs
- **Mục đích**: Test cho BotDetectionService
- **Test cases**: 4 tests
  - `GetFlaggedCode_WithBotDetection_ShouldReturnBotKind`
  - `GetFlaggedCode_WithNoBotDetection_ShouldReturnZero`
  - `GetFlaggedCode_WithEmptyDeviceNo_ShouldReturnTwo`
  - `GetFlaggedCode_WithEmptyUserAgent_ShouldReturnThree`

## Cách chạy Tests

### Sử dụng Visual Studio
1. Mở solution trong Visual Studio
2. Mở Test Explorer (Test > Test Explorer)
3. Chạy tất cả tests hoặc chạy từng test riêng lẻ

### Sử dụng Command Line
```bash
# Chạy tất cả tests
dotnet test

# Chạy tests với output chi tiết
dotnet test --verbosity normal

# Chạy tests với coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Dependencies

### NuGet Packages
- `xunit` (2.9.2) - Testing framework
- `xunit.runner.visualstudio` (2.8.2) - Test runner
- `Moq` (4.20.70) - Mocking framework
- `FluentAssertions` (6.12.0) - Assertion library
- `Microsoft.Extensions.DependencyInjection` (9.0.8)
- `Microsoft.Extensions.Configuration` (9.0.8)
- `Microsoft.Extensions.Logging` (9.0.8)

### Project References
- `FPS.AI.Powered.Device.Fingerprint.Consumer.Worker`
- `Device.FingerPrint.Model`
- `Device.FingerPrint.Core`
- `Device.FingerPrint`
- `Device.FingerPrint.Redis`
- `Device.FingerPrint.Queue`

## Test Coverage

Hiện tại đã có tests cho:
- ✅ DeviceService - 5 tests
- ✅ BotDetectionService - 4 tests
- ✅ SimpleTest - 1 test

**Tổng cộng: 9 tests**

## Các Test Cases Chính

### DeviceService Tests
- Test với device code hợp lệ (Base64 encoded JSON)
- Test với device code rỗng
- Test với device code null
- Test với Base64 không hợp lệ

### BotDetectionService Tests
- Test với bot detection
- Test không có bot detection
- Test với device number rỗng
- Test với user agent rỗng

## Best Practices

1. **Naming Convention**: Sử dụng pattern `MethodName_Scenario_ExpectedResult`
2. **Arrange-Act-Assert**: Mỗi test tuân theo pattern AAA
3. **Mocking**: Sử dụng Moq để mock dependencies
4. **Assertions**: Sử dụng FluentAssertions cho assertions rõ ràng
5. **Test Isolation**: Mỗi test độc lập và không phụ thuộc vào test khác

## Troubleshooting

### Lỗi thường gặp
1. **Namespace errors**: Đảm bảo using statements đúng
2. **Constructor errors**: Kiểm tra dependencies của service
3. **Property errors**: Kiểm tra tên properties trong models

### Debug Tests
```bash
# Chạy test với debug output
dotnet test --logger "console;verbosity=detailed"
```

## Contributing

Khi thêm tests mới:
1. Tạo file test trong thư mục Services/ tương ứng
2. Đặt tên class theo pattern `{ServiceName}Tests`
3. Đặt tên method theo pattern `{MethodName}_{Scenario}_{ExpectedResult}`
4. Sử dụng Moq cho mocking và FluentAssertions cho assertions
5. Chạy tests để đảm bảo pass

## Notes

- Một số services phức tạp (như RawTransactionService, MobileRawTransactionService) cần nhiều dependencies và có thể cần setup phức tạp hơn
- AppSettings có vấn đề với constructor trong test environment, cần mock configuration đúng cách
- Utilities class có vấn đề với namespace, cần sử dụng fully qualified name
