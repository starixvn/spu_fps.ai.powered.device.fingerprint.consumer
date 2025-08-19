FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src

COPY ["./src/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker", "FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/"]

COPY ["./src/Device.FingerPrint.Model", "Device.FingerPrint.Model/"]

COPY ["./src/Device.FingerPrint", "Device.FingerPrint/"]

COPY ["./src/Device.FingerPrint.Queue", "Device.FingerPrint.Queue/"]

COPY ["./src/Device.FingerPrint.Redis", "Device.FingerPrint.Redis/"]

COPY ["./src/Device.FingerPrint.Core", "Device.FingerPrint.Core/"]

COPY ["./src/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.csproj", "FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/"]

WORKDIR "/src/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker"

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2


RUN dotnet restore -v n

COPY ./src/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.dll"]
