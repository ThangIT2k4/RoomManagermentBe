FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/RoomManagerment.Messaging/RoomManagerment.Messaging.csproj", "BuildingBlocks/RoomManagerment.Messaging/"]
COPY ["Modules/Organization/Organization.Domain/Organization.Domain.csproj", "Modules/Organization/Organization.Domain/"]
COPY ["Modules/Organization/Organization.Application/Organization.Application.csproj", "Modules/Organization/Organization.Application/"]
COPY ["Modules/Organization/Organization.Infrastructure/Organization.Infrastructure.csproj", "Modules/Organization/Organization.Infrastructure/"]
COPY ["Modules/Organization/DAL/DatabaseGeneric/RoomManagerment.Organizations.csproj", "Modules/Organization/DAL/DatabaseGeneric/"]
COPY ["Modules/Organization/DAL/DatabaseSpecific/RoomManagerment.OrganizationsDBSpecific.csproj", "Modules/Organization/DAL/DatabaseSpecific/"]
COPY ["Modules/Organization/Organization.API/Organization.API.csproj", "Modules/Organization/Organization.API/"]

RUN dotnet restore "Modules/Organization/Organization.API/Organization.API.csproj"

COPY . .
WORKDIR "/src/Modules/Organization/Organization.API"
RUN dotnet build "./Organization.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Organization.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5014
ENV ASPNETCORE_HTTP_PORTS=5014
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Organization.API.dll"]
