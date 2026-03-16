FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Context là root RoomManagermentBe; ExternalApi nằm trong Api/, và reference BuildingBlocks
COPY ["Api/RoomManagerment.ExternalApi/RoomManagerment.ExternalApi.csproj", "Api/RoomManagerment.ExternalApi/"]
COPY ["BuildingBlocks/RoomManagerment.Messaging/RoomManagerment.Messaging.csproj", "BuildingBlocks/RoomManagerment.Messaging/"]
RUN dotnet restore "Api/RoomManagerment.ExternalApi/RoomManagerment.ExternalApi.csproj"

COPY . .
WORKDIR "/app/Api/RoomManagerment.ExternalApi"
RUN dotnet build "RoomManagerment.ExternalApi.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "RoomManagerment.ExternalApi.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "RoomManagerment.ExternalApi.dll"]
