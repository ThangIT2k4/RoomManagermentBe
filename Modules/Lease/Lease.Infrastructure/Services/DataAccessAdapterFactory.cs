using Lease.Application.Services;
using Microsoft.Extensions.Configuration;
using RoomManagerment.Lease.DatabaseSpecific;

namespace Lease.Infrastructure.Services;

public sealed class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("Lease")
        ?? throw new InvalidOperationException("Lease connection string not found.");

    public IDisposable CreateAdapter() => new DataAccessAdapter(_connectionString);
}

