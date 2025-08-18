# Unit Tests cho FPS.AI.Powered.Device.Fingerprint.Consumer.Worker

## Tổng quan

Dự án này chứa các Unit Test được viết bằng xUnit và NSubstitute cho hệ thống Device Fingerprint Consumer Worker. Các test được tổ chức theo cấu trúc chuyên nghiệp và bao phủ các thành phần chính của hệ thống.

**📊 Kết quả hiện tại: 42 test cases - Tất cả đều PASS ✅**

## Cấu trúc thư mục

```
FPS.AI.Powered.Device.Fingerprint.Consumer.Tests/
├── Services/
│   ├── BotDetectionServiceTests.cs      # Test cho BotDetectionService (8 tests)
│   ├── DeviceServiceTests.cs            # Test cho DeviceService (8 tests)
│   ├── UserAgentServiceTests.cs         # Test cho UserAgentService (6 tests)
│   └── InitServicesTests.cs             # Test cho InitServices (4 tests)
├── Core/
│   └── UtilitiesTests.cs                # Test cho Utilities class (10 tests)
├── Workers/
│   └── WorkerTests.cs                   # Test cho Worker background service (4 tests)
├── Middleware/
│   └── ExceptionMiddlewareTests.cs      # Test cho ExceptionMiddleware (2 tests)
└── README.md                            # File hướng dẫn này
```

## Công nghệ sử dụng

- **xUnit**: Framework testing chính
- **NSubstitute**: Mocking framework
- **FluentAssertions**: Thư viện assertion mạnh mẽ
- **.NET 9.0**: Target framework

## Cách chạy test

### Chạy tất cả test
```bash
dotnet test
```

### Chạy test với coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Chạy test cụ thể
```bash
dotnet test --filter "FullyQualifiedName~BotDetectionServiceTests"
```

### Chạy test theo namespace
```bash
dotnet test --filter "TestClass~Services"
```

## Mô tả các test

### 1. BotDetectionServiceTests (8 tests) ✅
Test các chức năng của BotDetectionService:
- `GetFlaggedCode`: Test việc xác định mã flagged cho bot detection

**Test cases bao gồm:**
- BotD null/empty
- Bot detection true/false
- Empty device number
- Edge cases và error handling

### 2. DeviceServiceTests (8 tests) ✅
Test các chức năng của DeviceService:
- `GetDeviceInfo`: Test việc lấy và chuyển đổi thông tin device
- Xử lý các trường hợp null, empty, invalid data
- Test việc decode Base64 và deserialize JSON

**Test cases bao gồm:**
- Device code null/empty
- Invalid Base64 encoding
- Exception handling
- Device info transformation

### 3. UserAgentServiceTests (6 tests) ✅
Test các chức năng của UserAgentService:
- `GetUserAgentInfo`: Test việc parse user agent string
- Device type detection (Desktop, Mobile, Tablet, Other)
- Client và OS information extraction

**Test cases bao gồm:**
- User agent null/empty
- Valid desktop user agent
- Valid mobile user agent
- Invalid user agent
- Exception handling
- MD5 hash generation

### 4. InitServicesTests (4 tests) ✅
Test các chức năng của InitServices:
- `RegisterService`: Test việc đăng ký tất cả services vào DI container
- Service lifetime management (Singleton)
- Dependency injection configuration

**Test cases bao gồm:**
- Service registration
- Singleton pattern verification
- Interface resolution
- Multiple registration calls
- Service dependency order

### 5. UtilitiesTests (10 tests) ✅
Test các chức năng của Utilities class:
- `GetMD5HashFromObject`: Test việc tạo MD5 hash từ object
- `IsValidJson`: Test việc validate JSON string

**Test cases bao gồm:**
- Valid/invalid JSON objects
- Valid/invalid JSON arrays
- Null/empty strings
- Whitespace handling
- Partial JSON strings
- Nested JSON structures
- MD5 hash generation from objects

### 6. WorkerTests (4 tests) ✅
Test các chức năng của Worker background service:
- Background service lifecycle
- Logging functionality
- Cancellation handling

**Test cases bao gồm:**
- Constructor validation
- ExecuteAsync with logging
- Cancellation token handling
- Logger enabled/disabled states
- Periodic logging verification

### 7. ExceptionMiddlewareTests (2 tests) ✅
Test các chức năng của ExceptionMiddleware:
- Exception handling và logging
- Middleware pipeline integration
- Error propagation control

**Test cases bao gồm:**
- Normal request flow
- Exception logging
- Exception suppression
- Custom exception handling
- Multiple exception scenarios
- Null context handling

## Best Practices được áp dụng

### 1. Naming Convention
- Test method names theo format: `MethodName_WhenCondition_ShouldExpectedResult`
- Ví dụ: `GetFlaggedCode_WhenBotDIsNull_ShouldReturnBrowserlessFlag`

### 2. Arrange-Act-Assert Pattern
Mỗi test method được chia thành 3 phần rõ ràng:
```csharp
[Fact]
public void MethodName_WhenCondition_ShouldExpectedResult()
{
    // Arrange - Chuẩn bị dữ liệu
    var input = "test";
    
    // Act - Thực hiện hành động
    var result = service.Method(input);
    
    // Assert - Kiểm tra kết quả
    result.Should().Be("expected");
}
```

### 3. FluentAssertions
Sử dụng FluentAssertions cho assertions dễ đọc:
```csharp
result.Should().NotBeNull();
result.Should().Be("expected");
result.Should().BeEmpty();
result.Should().BeTrue();
```

### 4. Test Categories
- **Unit Tests**: Test từng method riêng biệt
- **Integration Tests**: Test interaction giữa các components
- **Edge Cases**: Test các trường hợp đặc biệt (null, empty, invalid)
- **Exception Handling**: Test error scenarios
- **Service Registration**: Test DI container configuration

## Coverage

Các test được thiết kế để bao phủ:
- ✅ Happy path scenarios
- ✅ Error handling
- ✅ Edge cases (null, empty, invalid data)
- ✅ Bot detection logic
- ✅ Device info transformation
- ✅ User agent parsing
- ✅ Service registration
- ✅ Background service lifecycle
- ✅ Middleware exception handling
- ✅ Input validation
- ✅ JSON validation
- ✅ MD5 hash generation

## Troubleshooting

### Lỗi thường gặp

1. **Test không chạy được**
   - Kiểm tra project references
   - Đảm bảo target framework đúng (.NET 9.0)
   - Kiểm tra NuGet packages

2. **Assertion failures**
   - Kiểm tra expected values
   - Verify data types
   - Kiểm tra null handling

3. **Mock setup issues**
   - Kiểm tra interface implementations
   - Verify method signatures
   - Kiểm tra dependency injection

### Debug test
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Contributing

Khi thêm test mới:
1. Tuân thủ naming convention
2. Sử dụng Arrange-Act-Assert pattern
3. Test cả happy path và edge cases
4. Sử dụng FluentAssertions cho assertions
5. Thêm comments cho test cases phức tạp
6. Đảm bảo test coverage đầy đủ
7. Mock external dependencies
8. Test exception scenarios

## Build và Deploy

### Build project
```bash
dotnet build
```

### Clean và rebuild
```bash
dotnet clean
dotnet build
```

### Run tests với verbose output
```bash
dotnet test --verbosity normal
```

### Run tests với coverage report
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

## Kết quả Test Summary

```
Test summary: total: 42, failed: 0, succeeded: 42, skipped: 0, duration: 6.1s
Build succeeded with 4 warning(s) in 7.3s
```

**🎉 Tất cả 42 test cases đã pass thành công!**
