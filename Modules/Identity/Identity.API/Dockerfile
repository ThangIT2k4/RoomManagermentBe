FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["Modules/Identity/Identity.API/Identity.API.csproj", "Modules/Identity/Identity.API/"]
COPY ["Modules/Identity/Identity.Domain/Identity.Domain.csproj", "Modules/Identity/Identity.Domain/"]
COPY ["Modules/Identity/Identity.Application/Identity.Application.csproj", "Modules/Identity/Identity.Application/"]
COPY ["Modules/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Modules/Identity/Identity.Infrastructure/"]
COPY ["Modules/Identity/DAL/DatabaseGeneric/RoomManagerment.Identity.csproj", "Modules/Identity/DAL/DatabaseGeneric/"]
COPY ["Modules/Identity/DAL/DatabaseSpecific/RoomManagerment.IdentityDBSpecific.csproj", "Modules/Identity/DAL/DatabaseSpecific/"]
COPY ["BuildingBlocks/RoomManagerment.Messaging/RoomManagerment.Messaging.csproj", "BuildingBlocks/RoomManagerment.Messaging/"]

RUN dotnet restore "Modules/Identity/Identity.API/Identity.API.csproj"

COPY . .
WORKDIR "/app/Modules/Identity/Identity.API"
RUN dotnet build "Identity.API.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Identity.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Identity.API.dll"]
