FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Modules/Property/Property.Domain/Property.Domain.csproj", "Modules/Property/Property.Domain/"]
COPY ["Modules/Property/Property.Application/Property.Application.csproj", "Modules/Property/Property.Application/"]
COPY ["Modules/Property/Property.Infrastructure/Property.Infrastructure.csproj", "Modules/Property/Property.Infrastructure/"]
COPY ["Modules/Property/DAL/DatabaseGeneric/RoomManagerment.Property.csproj", "Modules/Property/DAL/DatabaseGeneric/"]
COPY ["Modules/Property/DAL/DatabaseSpecific/RoomManagerment.PropertyDBSpecific.csproj", "Modules/Property/DAL/DatabaseSpecific/"]
COPY ["Modules/Property/Property.API/Property.API.csproj", "Modules/Property/Property.API/"]

RUN dotnet restore "Modules/Property/Property.API/Property.API.csproj"

COPY . .
WORKDIR "/src/Modules/Property/Property.API"
RUN dotnet build "./Property.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Property.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5007
ENV ASPNETCORE_HTTP_PORTS=5007
COPY --from=publish /app/publish .
RUN adduser --disabled-password --uid 10001 --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Property.API.dll"]
