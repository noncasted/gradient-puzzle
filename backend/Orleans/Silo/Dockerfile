FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update
RUN apt-get install -y curl
USER $APP_UID
WORKDIR /app
EXPOSE 9120

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Orleans/Silo/Silo.csproj", "Orleans/Silo/"]
COPY ["Aspire/ServiceDefaults/ServiceDefaults.csproj", "Aspire/ServiceDefaults/"]
COPY ["Common/Extensions/Extensions.csproj", "Common/Extensions/"]
COPY ["Users/Grains/Grains.csproj", "Users/Grains/"]
COPY ["Users/Interfaces/Interfaces.csproj", "Users/Interfaces/"]
RUN dotnet restore "Orleans/Silo/Silo.csproj"
COPY . .
WORKDIR "/src/Orleans/Silo"
RUN dotnet build "Silo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Silo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orleans.Silo.dll"]
