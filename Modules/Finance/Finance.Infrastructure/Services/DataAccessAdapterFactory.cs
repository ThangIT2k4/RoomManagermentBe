using Finance.Application.Services;
using Microsoft.Extensions.Configuration;
using RoomManagerment.Finance.DatabaseSpecific;

namespace Finance.Infrastructure.Services;

public sealed class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("Finance")
        ?? throw new InvalidOperationException("Finance connection string not found.");

    public IDisposable CreateAdapter() => new DataAccessAdapter(_connectionString);
}

