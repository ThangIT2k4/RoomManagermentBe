FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Modules/Notification/Notification.API/Notification.API.csproj", "Modules/Notification/Notification.API/"]
COPY ["Modules/Notification/Notification.Application/Notification.Application.csproj", "Modules/Notification/Notification.Application/"]
COPY ["Modules/Notification/Notification.Domain/Notification.Domain.csproj", "Modules/Notification/Notification.Domain/"]
COPY ["Modules/Notification/Notification.Infrastructure/Notification.Infrastructure.csproj", "Modules/Notification/Notification.Infrastructure/"]
COPY ["Modules/Notification/DAL/DatabaseGeneric/RoomManagerment.Notification.csproj", "Modules/Notification/DAL/DatabaseGeneric/"]
COPY ["Modules/Notification/DAL/DatabaseSpecific/RoomManagerment.NotificationDBSpecific.csproj", "Modules/Notification/DAL/DatabaseSpecific/"]
RUN dotnet restore "Modules/Notification/Notification.API/Notification.API.csproj"
COPY . .
WORKDIR "/src/Modules/Notification/Notification.API"
RUN dotnet build "./Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5001
ENV ASPNETCORE_HTTP_PORTS=5001
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Notification.API.dll"]
