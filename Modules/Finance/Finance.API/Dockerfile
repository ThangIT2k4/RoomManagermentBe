FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuildingBlocks/RoomManagerment.Messaging/RoomManagerment.Messaging.csproj", "BuildingBlocks/RoomManagerment.Messaging/"]
COPY ["Modules/Finance/Finance.Domain/Finance.Domain.csproj", "Modules/Finance/Finance.Domain/"]
COPY ["Modules/Finance/Finance.Application/Finance.Application.csproj", "Modules/Finance/Finance.Application/"]
COPY ["Modules/Finance/Finance.Infrastructure/Finance.Infrastructure.csproj", "Modules/Finance/Finance.Infrastructure/"]
COPY ["Modules/Finance/DAL/DatabaseGeneric/RoomManagerment.Finance.csproj", "Modules/Finance/DAL/DatabaseGeneric/"]
COPY ["Modules/Finance/DAL/DatabaseSpecific/RoomManagerment.FinanceDBSpecific.csproj", "Modules/Finance/DAL/DatabaseSpecific/"]
COPY ["Modules/Finance/Finance.API/Finance.API.csproj", "Modules/Finance/Finance.API/"]

RUN dotnet restore "Modules/Finance/Finance.API/Finance.API.csproj"

COPY . .
WORKDIR "/src/Modules/Finance/Finance.API"
RUN dotnet build "./Finance.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Finance.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5003
ENV ASPNETCORE_HTTP_PORTS=5003
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Finance.API.dll"]
