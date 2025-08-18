FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine-25h2 AS builder

WORKDIR /

COPY ["./FPS.AI.Powered.Device.Fingerprint.Consumer.Worker", "FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/"]

COPY ["./Device.FingerPrint.Model", "Device.FingerPrint.Model/"]

COPY ["./Device.FingerPrint", "Device.FingerPrint/"]

COPY ["./Device.FingerPrint.Queue", "Device.FingerPrint.Queue/"]

COPY ["./Device.FingerPrint.Redis", "Device.FingerPrint.Redis/"]

COPY ["./Device.FingerPrint.Core", "Device.FingerPrint.Core/"]

COPY ["./FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.csproj", "FPS.AI.Powered.Device.Fingerprint.Consumer.Worker/"]

WORKDIR "/FPS.AI.Powered.Device.Fingerprint.Consumer.Worker"

RUN dotnet nuget add source http://nuget.nexdev.net/api/v2

RUN dotnet restore -v n

COPY ./FPS.AI.Powered.Device.Fingerprint.Consumer.Worker .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine-25h2 AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Device.FingerPrint.QueueConsumer.dll"]
