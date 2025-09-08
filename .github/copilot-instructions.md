# Device Fingerprint Consumer - AI Coding Instructions

## Architecture Overview

This is a **device fingerprint processing system** that consumes messages from RabbitMQ queues, processes device identification data, and stores transaction records in MySQL databases. The system uses:

- **MassTransit** for RabbitMQ message consumption with batch processing
- **Multi-environment configuration** (LOCAL, UAT, STG, PROD, KLPROD, SimpleKL) via appsettings files  
- **Custom Base64/encryption** for device fingerprint decoding using project-specific keys
- **Redis caching** for performance optimization
- **MySQL databases** for transaction storage and analytics

## Key Components & Data Flow

### Core Message Processing Pipeline
1. **MessageQueue** model receives JSON from RabbitMQ (see `src/Device.FingerPrint.QueueConsumer/Models/MessageQueue.cs`)
2. **Consumer** processes batches via MassTransit (`src/Device.FingerPrint.QueueConsumer/Handlers/Consumer.cs`)
3. **DeviceService** decodes Base64-encoded device info using custom key (`di` field)
4. **RawTransactionService** transforms messages to database format
5. **TransactionService** persists to MySQL

### Critical Configuration Patterns
- **Environment-specific settings**: Each environment has dedicated appsettings files (e.g., `appsettings.PROD.json`, `appsettings.UAT.Mobile.json`)
- **Consumer settings**: Batch processing controlled by `Consumer:Masstransit:BatchSizeLimit/BatchTimeLimit/BatchConcurrencyLimit`
- **Mobile vs Web**: `Consumer:IsMobileConsumer` flag determines processing path

## JSON Serialization Requirements

**CRITICAL**: The project uses **Newtonsoft.Json** throughout but models previously used **System.Text.Json** attributes. Always use:

```csharp
[JsonProperty("propertyName")] // ✅ Correct - works with JsonConvert.DeserializeObject
[JsonPropertyName("propertyName")] // ❌ Wrong - ignored by Newtonsoft.Json
```

**Standard JSON settings**: Use `JsonSerializerSettingsFactory.Instance` for consistent camelCase serialization.

## Device Fingerprint Decoding

Device info (`di` field) requires custom Base64 decoding:

```csharp
// Always use the project-specific key from DeviceService
private const string base64Key = "wFKfVDGZqRbyYCBi6IveMaQAh1ntWcP8ujT/U7r2xXs+J0L=4d3zEHSo5glN9kOpm";
string decoded = Base64.Decode(di, base64Key);
var deviceInfo = JsonConvert.DeserializeObject<DeviceInfoModel>(decoded, JsonSerializerSettingsFactory.Instance);
```

## Testing Patterns

- **Unit tests** use Moq for service mocking
- **Consumer tests** mock `ConsumeContext<Batch<MessageQueue>>` for MassTransit
- **Configuration tests** use `ConfigurationBuilder().AddInMemoryCollection()`
- **DeviceService tests** validate Base64 decoding with various edge cases

## Development Commands

```bash
# Build entire solution
dotnet build fps.ai.powered.device.fingerprint.consumer.sln

# Run specific project (QueueConsumer is the main service)
dotnet run --project src/Device.FingerPrint.QueueConsumer

# Run all tests
dotnet test

# Run specific test project
dotnet test test/Device.FingerPrint.QueueConsumer.Tests
```

## Project Dependencies

- **Core**: `Device.FingerPrint.Core` - shared utilities, encryption, configuration
- **Model**: `Device.FingerPrint.Model` - data models, enums
- **Queue**: `Device.FingerPrint.Queue` - RabbitMQ abstractions
- **QueueConsumer**: Main service consuming messages
- **Redis**: Caching layer

## Common Debugging Scenarios

- **Serialization issues**: Check Newtonsoft vs System.Text.Json attribute usage
- **Base64 decode failures**: Verify custom key usage in `DeviceService`
- **Consumer not processing**: Check `Consumer:IsConsume` configuration flag
- **Environment config**: Ensure correct appsettings file is loaded for target environment
- **Batch processing**: Verify MassTransit batch configuration limits

## Model Validation Patterns

When adding properties to `MessageQueue` or other models:
1. Compare with actual JSON structure received
2. Add `[JsonProperty("camelCaseName")]` attributes
3. Use nullable types (`string?`, `DateTime?`) for optional fields
4. Test with actual message samples from different environments
