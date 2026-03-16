FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Context là root RoomManagermentBe; Gateway nằm trong Api/
COPY ["Api/RoomManagerment.Gateway/RoomManagerment.Gateway.csproj", "Api/RoomManagerment.Gateway/"]
RUN dotnet restore "Api/RoomManagerment.Gateway/RoomManagerment.Gateway.csproj"

COPY . .
WORKDIR "/app/Api/RoomManagerment.Gateway"
RUN dotnet build "RoomManagerment.Gateway.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "RoomManagerment.Gateway.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "RoomManagerment.Gateway.dll"]
