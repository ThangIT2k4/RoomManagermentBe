using Auth.Application.Services;
using Microsoft.Extensions.Configuration;
using RoomManagerment.Auth.DatabaseSpecific;

namespace Auth.Infrastructure.Services;

public sealed class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("Auth")
        ?? throw new InvalidOperationException("Auth connection string not found.");

    public IDisposable CreateAdapter() => new DataAccessAdapter(_connectionString);
}

