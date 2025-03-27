FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

ENV TELEGRAM_TOKEN=$TELEGRAM_TOKEN
ENV BUILD_URL=$BUILD_URL

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Gateway/Gateway/Gateway.csproj", "Gateway/Gateway/"]
COPY ["Aspire/ServiceDefaults/ServiceDefaults.csproj", "Aspire/ServiceDefaults/"]
COPY ["Gateway/Telegram/Telegram.csproj", "Gateway/Telegram/"]
COPY ["Common/Extensions/Extensions.csproj", "Common/Extensions/"]
COPY ["Users/Endpoints/Endpoints.csproj", "Users/Endpoints/"]
COPY ["Shared/Protocol/Protocol.csproj", "Shared/Protocol/"]
COPY ["Users/Interfaces/Interfaces.csproj", "Users/Interfaces/"]

RUN dotnet restore "Gateway/Gateway/Gateway.csproj"
COPY . .
WORKDIR "/src/Gateway/Gateway"

RUN dotnet build "Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]
