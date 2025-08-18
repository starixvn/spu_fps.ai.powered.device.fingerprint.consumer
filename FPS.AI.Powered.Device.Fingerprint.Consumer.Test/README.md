# Unit Tests cho FPS.AI.Powered.Device.Fingerprint.Consumer

## Tổng quan

Project này chứa các unit test cho tất cả các service và component trong FPS.AI.Powered.Device.Fingerprint.Consumer solution.

## Cấu trúc Test

### Services Tests
- **RawTransactionServiceTests.cs**: Test cho RawTransactionService
- **BotDetectionServiceTests.cs**: Test cho BotDetectionService  
- **DeviceServiceTests.cs**: Test cho DeviceService
- **UserAgentServiceTests.cs**: Test cho UserAgentService
- **MobileRawTransactionServiceTests.cs**: Test cho MobileRawTransactionService

### Handlers Tests
- **ConsumerTests.cs**: Test cho Consumer class

### Core Tests
- **CryptoTests.cs**: Test cho Crypto service
- **AppSettingsTests.cs**: Test cho AppSettings class

### Utilities Tests
- **UtilitiesTests.cs**: Test cho Utilities class

## Cách chạy Test

### Sử dụng Visual Studio
1. Mở solution trong Visual Studio
2. Mở Test Explorer (Test > Test Explorer)
3. Click "Run All" để chạy tất cả test

### Sử dụng Command Line
```bash
# Chạy tất cả test
dotnet test

# Chạy test với coverage
dotnet test --collect:"XPlat Code Coverage"

# Chạy test cụ thể
dotnet test --filter "FullyQualifiedName~RawTransactionServiceTests"

# Chạy test với output chi tiết
dotnet test --logger "console;verbosity=detailed"
```

### Sử dụng PowerShell
```powershell
# Chạy test
dotnet test

# Chạy test với coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Các Test Case Chính

### RawTransactionService
- TransformRawTransaction với device hợp lệ
- TransformRawTransaction với device code rỗng
- TransformRawTransaction với device ID không hợp lệ
- InsertOrUpdateEverTagging
- LogTransactions

### BotDetectionService
- GetFlaggedCode với bot detection
- GetFlaggedCode với browserless detection
- TransformBotdModel với dữ liệu hợp lệ
- GetBotDetectionTrue với detection data

### DeviceService
- GetDeviceInfo với device code hợp lệ
- GetDeviceInfo với device code rỗng/null
- Transform device info với BotD data
- Transform device info với screen data

### UserAgentService
- GetUserAgentInfo với desktop user agent
- GetUserAgentInfo với mobile user agent
- GetUserAgentInfo với tablet user agent
- Xử lý các loại browser khác nhau

### MobileRawTransactionService
- TransformRawTransaction với mobile data hợp lệ
- Xử lý device codes rỗng
- Xử lý IPv6 addresses
- Xử lý emulator detection

### Consumer
- Consume với messages hợp lệ
- Consume với empty batch
- Consume với consume disabled
- Xử lý mobile app transactions
- Xử lý invalid accounts

### Crypto
- Encrypt/Decrypt với text hợp lệ
- Encrypt/Decrypt với special characters
- Encrypt/Decrypt với unicode characters
- GetMD5Hash với các input khác nhau

### AppSettings
- Đọc các configuration values
- GetValue với các kiểu dữ liệu khác nhau
- Xử lý missing keys
- Xử lý invalid values

### Utilities
- GetMD5HashFromObject
- IsValidJson với các input khác nhau
- Xử lý null và empty inputs

## Dependencies

Project test sử dụng các package sau:
- **xUnit**: Framework test
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library
- **Microsoft.Extensions.DependencyInjection**: DI container
- **Microsoft.Extensions.Configuration**: Configuration
- **Microsoft.Extensions.Logging**: Logging

## Best Practices

1. **Arrange-Act-Assert**: Tất cả test đều tuân theo pattern AAA
2. **Mocking**: Sử dụng Moq để mock dependencies
3. **Fluent Assertions**: Sử dụng FluentAssertions cho readable assertions
4. **Test Naming**: Tên test mô tả rõ ràng scenario và expected result
5. **Edge Cases**: Test các trường hợp edge cases và error conditions
6. **Isolation**: Mỗi test độc lập và không phụ thuộc vào test khác

## Coverage

Các test bao phủ:
- ✅ Business logic trong services
- ✅ Error handling
- ✅ Edge cases
- ✅ Configuration handling
- ✅ Data transformation
- ✅ Validation logic

## Troubleshooting

### Lỗi thường gặp

1. **Missing Dependencies**: Đảm bảo tất cả project references được thêm vào
2. **Configuration Issues**: Kiểm tra appsettings.json và configuration setup
3. **Mock Setup**: Đảm bảo mock được setup đúng cách
4. **Async Tests**: Sử dụng async/await cho async operations

### Debug Tests

```bash
# Chạy test với debug
dotnet test --logger "console;verbosity=detailed" --verbosity normal
```

## Contributing

Khi thêm test mới:
1. Tuân theo naming convention hiện tại
2. Sử dụng Arrange-Act-Assert pattern
3. Thêm test cho edge cases
4. Cập nhật documentation nếu cần
