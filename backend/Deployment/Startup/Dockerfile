FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Deployment/Startup/Startup.csproj", "Deployment/Startup/"]
COPY ["Aspire/ServiceDefaults/ServiceDefaults.csproj", "Aspire/ServiceDefaults/"]
COPY ["Common/Extensions/Extensions.csproj", "Common/Extensions/"]

RUN dotnet restore "Deployment/Startup/Startup.csproj"
COPY . .
WORKDIR "/src/Deployment/Startup"
RUN dotnet build "Startup.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Startup.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Deployment.Startup.dll"]
