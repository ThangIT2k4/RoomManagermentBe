using Microsoft.Extensions.Configuration;
using Property.Application.Services;
using RoomManagerment.Property.DatabaseSpecific;

namespace Property.Infrastructure.Services;

public sealed class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("Property")
        ?? throw new InvalidOperationException("Property connection string not found.");

    public IDisposable CreateAdapter() => new DataAccessAdapter(_connectionString);
}

