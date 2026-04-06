using CRM.Application.Services;
using Microsoft.Extensions.Configuration;
using RoomManagerment.CRM.DatabaseSpecific;

namespace CRM.Infrastructure.Services;

public sealed class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("CRM")
        ?? throw new InvalidOperationException("CRM connection string not found.");

    public IDisposable CreateAdapter() => new DataAccessAdapter(_connectionString);
}

