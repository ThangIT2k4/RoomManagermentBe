FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/RoomManagerment.Messaging/RoomManagerment.Messaging.csproj", "BuildingBlocks/RoomManagerment.Messaging/"]
COPY ["Modules/Organization/DAL/DatabaseGeneric/RoomManagerment.Organizations.csproj", "Modules/Organization/DAL/DatabaseGeneric/"]
COPY ["Modules/Organization/DAL/DatabaseSpecific/RoomManagerment.OrganizationsDBSpecific.csproj", "Modules/Organization/DAL/DatabaseSpecific/"]
COPY ["Modules/Auth/Auth.Domain/Auth.Domain.csproj", "Modules/Auth/Auth.Domain/"]
COPY ["Modules/Auth/Auth.Application/Auth.Application.csproj", "Modules/Auth/Auth.Application/"]
COPY ["Modules/Auth/Auth.Infrastructure/Auth.Infrastructure.csproj", "Modules/Auth/Auth.Infrastructure/"]
COPY ["Modules/Auth/DAL/DatabaseGeneric/RoomManagerment.Auth.csproj", "Modules/Auth/DAL/DatabaseGeneric/"]
COPY ["Modules/Auth/DAL/DatabaseSpecific/RoomManagerment.AuthDBSpecific.csproj", "Modules/Auth/DAL/DatabaseSpecific/"]
COPY ["Modules/Auth/Auth.API/Auth.API.csproj", "Modules/Auth/Auth.API/"]

RUN dotnet restore "Modules/Auth/Auth.API/Auth.API.csproj"

COPY . .
WORKDIR "/src/Modules/Auth/Auth.API"
RUN dotnet build "./Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5002
ENV ASPNETCORE_HTTP_PORTS=5002
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Auth.API.dll"]
