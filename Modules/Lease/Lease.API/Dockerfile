FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/RoomManagerment.Messaging/RoomManagerment.Messaging.csproj", "BuildingBlocks/RoomManagerment.Messaging/"]
COPY ["Modules/Lease/Lease.Domain/Lease.Domain.csproj", "Modules/Lease/Lease.Domain/"]
COPY ["Modules/Lease/Lease.Application/Lease.Application.csproj", "Modules/Lease/Lease.Application/"]
COPY ["Modules/Lease/Lease.Infrastructure/Lease.Infrastructure.csproj", "Modules/Lease/Lease.Infrastructure/"]
COPY ["Modules/Lease/DAL/DatabaseGeneric/RoomManagerment.Lease.csproj", "Modules/Lease/DAL/DatabaseGeneric/"]
COPY ["Modules/Lease/DAL/DatabaseSpecific/RoomManagerment.LeaseDBSpecific.csproj", "Modules/Lease/DAL/DatabaseSpecific/"]
COPY ["Modules/Lease/Lease.API/Lease.API.csproj", "Modules/Lease/Lease.API/"]

RUN dotnet restore "Modules/Lease/Lease.API/Lease.API.csproj"

COPY . .
WORKDIR "/src/Modules/Lease/Lease.API"
RUN dotnet build "./Lease.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Lease.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5008
ENV ASPNETCORE_HTTP_PORTS=5008
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Lease.API.dll"]
