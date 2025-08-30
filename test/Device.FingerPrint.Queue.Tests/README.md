# Device.FingerPrint.Queue.Tests

Unit tests for the Device.FingerPrint.Queue project.

## Overview

This project contains unit tests for the Device.FingerPrint.Queue library, which provides queue management functionality using EasyNetQ for RabbitMQ integration.

## Test Framework

- **xUnit**: Primary testing framework
- **FluentAssertions**: Assertion library for readable test assertions
- **Moq**: Mocking framework for creating test doubles

## Test Coverage

### EasyNetQConnector Tests
- Constructor parameter validation
- Bus creation and management
- Exchange declaration
- Queue declaration
- Routing key configuration
- Binding operations
- Resource disposal
- Fluent interface pattern

## Running Tests

```bash
dotnet test
```

## Dependencies

- Device.FingerPrint.Core
- Device.FingerPrint.Model
- Device.FingerPrint.Queue
- EasyNetQ
- xUnit
- FluentAssertions
- Moq

## Project Structure

```
Device.FingerPrint.Queue.Tests/
├── EasyNetQConnectorTests.cs    # Tests for EasyNetQ connector
├── Device.FingerPrint.Queue.Tests.csproj
├── Directory.Build.props
├── Directory.Build.targets
├── GlobalUsings.cs
└── README.md
```
