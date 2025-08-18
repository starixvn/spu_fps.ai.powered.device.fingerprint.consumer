FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine-25h2 AS builder

WORKDIR /src

# Copy solution file first for better layer caching
COPY ["fps.ai.powered.device.fingerprint.consumer.sln", "./"]

# Copy all project files
COPY ["src/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.csproj", "src/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/"]
COPY ["src/Device.FingerPrint.Model/Device.FingerPrint.Model.csproj", "src/Device.FingerPrint.Model/"]
COPY ["src/Device.FingerPrint/Device.FingerPrint.csproj", "src/Device.FingerPrint/"]
COPY ["src/Device.FingerPrint.Queue/Device.FingerPrint.Queue.csproj", "src/Device.FingerPrint.Queue/"]
COPY ["src/Device.FingerPrint.Redis/Device.FingerPrint.Redis.csproj", "src/Device.FingerPrint.Redis/"]
COPY ["src/Device.FingerPrint.Core/Device.FingerPrint.Core.csproj", "src/Device.FingerPrint.Core/"]
COPY ["src/FPS.Device.Fingerprint.Service/FPS.Device.Fingerprint.Service.csproj", "src/FPS.Device.Fingerprint.Service/"]
COPY ["src/FPS.Device.Fingerprint.DataAccess/FPS.Device.Fingerprint.DataAccess.csproj", "src/FPS.Device.Fingerprint.DataAccess/"]

# Add NuGet source and restore packages
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore "src/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/src/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker"
RUN dotnet build "FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.csproj" -c Release -o /app/build
RUN dotnet publish "FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine-25h2 AS final

WORKDIR /app

# Copy published application
COPY --from=builder /app/publish .

# Copy App_Data folder
COPY --from=builder /src/src/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/App_Data ./App_Data

# Set timezone
ENV TZ="America/La_Paz"

# Add health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD dotnet FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.dll --health-check || exit 1

# Set the entry point to the correct assembly
ENTRYPOINT ["dotnet", "FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.dll"]
