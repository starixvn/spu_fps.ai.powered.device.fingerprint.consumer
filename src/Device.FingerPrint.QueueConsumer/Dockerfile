FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src

COPY ["./src/Device.FingerPrint.QueueConsumer", "Device.FingerPrint.QueueConsumer/"]

COPY ["./src/Device.FingerPrint.Model", "Device.FingerPrint.Model/"]

COPY ["./src/Device.FingerPrint", "Device.FingerPrint/"]

COPY ["./src/Device.FingerPrint.Queue", "Device.FingerPrint.Queue/"]

COPY ["./src/Device.FingerPrint.Core", "Device.FingerPrint.Core/"]

COPY ["./src/Device.FingerPrint.QueueConsumer/Device.FingerPrint.QueueConsumer.csproj", "Device.FingerPrint.QueueConsumer/"]

WORKDIR "/src/Device.FingerPrint.QueueConsumer"

# Add custom NuGet source
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

RUN dotnet restore -v n

COPY ./src/Device.FingerPrint.QueueConsumer .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

# Create a non-root user and group
RUN addgroup -g 1001 -S appuser && \
    adduser -S -D -H -u 1001 -g appuser -s /sbin/nologin appuser

WORKDIR /app

# Copy application files and set ownership to appuser
COPY --from=builder /app .
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Device.FingerPrint.QueueConsumer.dll"]
