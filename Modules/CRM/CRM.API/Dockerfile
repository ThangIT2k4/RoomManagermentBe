FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/RoomManagerment.Messaging/RoomManagerment.Messaging.csproj", "BuildingBlocks/RoomManagerment.Messaging/"]
COPY ["Modules/CRM/CRM.Domain/CRM.Domain.csproj", "Modules/CRM/CRM.Domain/"]
COPY ["Modules/CRM/CRM.Application/CRM.Application.csproj", "Modules/CRM/CRM.Application/"]
COPY ["Modules/CRM/CRM.Infrastructure/CRM.Infrastructure.csproj", "Modules/CRM/CRM.Infrastructure/"]
COPY ["Modules/CRM/DAL/DatabaseGeneric/RoomManagerment.CRM.csproj", "Modules/CRM/DAL/DatabaseGeneric/"]
COPY ["Modules/CRM/DAL/DatabaseSpecific/RoomManagerment.CRMDBSpecific.csproj", "Modules/CRM/DAL/DatabaseSpecific/"]
COPY ["Modules/CRM/CRM.API/CRM.API.csproj", "Modules/CRM/CRM.API/"]

RUN dotnet restore "Modules/CRM/CRM.API/CRM.API.csproj"

COPY . .
WORKDIR "/src/Modules/CRM/CRM.API"
RUN dotnet build "./CRM.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CRM.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5005
ENV ASPNETCORE_HTTP_PORTS=5005
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "CRM.API.dll"]
