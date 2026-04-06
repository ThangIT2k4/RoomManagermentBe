using Microsoft.Extensions.Configuration;
using Organization.Application.Services;
using RoomManagerment.Organizations.DatabaseSpecific;

namespace Organization.Infrastructure.Services;

public sealed class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("Organization")
        ?? throw new InvalidOperationException("Organization connection string not found.");

    public IDisposable CreateAdapter() => new DataAccessAdapter(_connectionString);
}

