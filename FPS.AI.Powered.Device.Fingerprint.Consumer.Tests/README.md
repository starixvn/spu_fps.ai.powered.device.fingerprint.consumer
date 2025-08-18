# Unit Tests cho FPS.AI.Powered.Device.Fingerprint.Consumer.Worker

## Tổng quan

Dự án này chứa các Unit Test được viết bằng xUnit và NSubstitute cho hệ thống Device Fingerprint Consumer Worker. Các test được tổ chức theo cấu trúc chuyên nghiệp và bao phủ các thành phần chính của hệ thống.

## Cấu trúc thư mục

```
FPS.AI.Powered.Device.Fingerprint.Consumer.Tests/
├── Services/
│   ├── BotDetectionServiceTests.cs      # Test cho BotDetectionService
│   └── DeviceServiceTests.cs            # Test cho DeviceService
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

### 1. BotDetectionServiceTests
Test các chức năng của BotDetectionService:
- `GetFlaggedCode`: Test việc xác định mã flagged cho bot detection
- `TransformBotdModel`: Test việc chuyển đổi BotD model từ message queue
- `GetBotDetectionTrue`: Test việc lọc các detection có bot = true

**Test cases bao gồm:**
- BotD null/empty
- Bot detection true/false
- Empty device number
- Valid/invalid Base64 JSON
- Edge cases và error handling

### 2. DeviceServiceTests
Test các chức năng của DeviceService:
- `GetDeviceInfo`: Test việc lấy và chuyển đổi thông tin device
- Xử lý các trường hợp null, empty, invalid data
- Test việc decode Base64 và deserialize JSON

**Test cases bao gồm:**
- Device code null/empty/"null"/"NULL"
- Valid device info với BotD
- Invalid Base64 encoding
- Exception handling
- Device info transformation



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

## Coverage

Các test được thiết kế để bao phủ:
- ✅ Happy path scenarios
- ✅ Error handling
- ✅ Edge cases (null, empty, invalid data)
- ✅ Bot detection logic
- ✅ Device info transformation
- ✅ Input validation

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
