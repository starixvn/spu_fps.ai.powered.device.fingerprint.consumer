FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine-25h2 AS builder

WORKDIR /src

COPY ["./src/Device.FingerPrint.QueueConsumer", "Device.FingerPrint.QueueConsumer/"]

COPY ["./src/Device.FingerPrint.Model", "Device.FingerPrint.Model/"]

COPY ["./src/Device.FingerPrint", "Device.FingerPrint/"]

COPY ["./src/Device.FingerPrint.Queue", "Device.FingerPrint.Queue/"]

COPY ["./src/Device.FingerPrint.Redis", "Device.FingerPrint.Redis/"]

COPY ["./src/Core/Device.FingerPrint.Core", "Core/Device.FingerPrint.Core/"]

COPY ["./src/Device.FingerPrint.QueueConsumer/Device.FingerPrint.QueueConsumer.csproj", "Device.FingerPrint.QueueConsumer/"]

WORKDIR "/src/Device.FingerPrint.QueueConsumer"

RUN dotnet nuget add source http://nuget.nexdev.net/api/v2

RUN dotnet restore -v n

COPY ./src/Device.FingerPrint.QueueConsumer .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine-25h2 AS base

WORKDIR /app

COPY --from=builder /app .

ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Device.FingerPrint.QueueConsumer.dll"]
