FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /app

COPY ["Modules/Identity/Identity.API/Identity.API.csproj", "Modules/Identity/Identity.API/"]
COPY ["Modules/Identity/Identity.Domain/Identity.Domain.csproj", "Modules/Identity/Identity.Domain/"]
COPY ["Modules/Identity/Identity.Application/Identity.Application.csproj", "Modules/Identity/Identity.Application/"]
COPY ["Modules/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Modules/Identity/Identity.Infrastructure/"]
COPY ["Modules/Identity/DAL/DatabaseGeneric/RoomManagerment.Identity.csproj", "Modules/Identity/DAL/DatabaseGeneric/"]
COPY ["Modules/Identity/DAL/DatabaseSpecific/RoomManagerment.IdentityDBSpecific.csproj", "Modules/Identity/DAL/DatabaseSpecific/"]

RUN dotnet restore "Modules/Identity/Identity.API/Identity.API.csproj"

COPY . .
WORKDIR "/app/Modules/Identity/Identity.API"
RUN dotnet build "Identity.API.csproj" -c Release -o /app/build

FROM builder AS publish
RUN dotnet publish "Identity.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.API.dll"]