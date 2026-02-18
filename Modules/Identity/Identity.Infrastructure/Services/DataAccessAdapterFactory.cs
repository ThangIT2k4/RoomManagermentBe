using Identity.Application.Services;
using Microsoft.Extensions.Configuration;
using RoomManagerment.Identity.DatabaseSpecific;

namespace Identity.Infrastructure.Services;

public class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("Identity") ?? 
                                                throw new InvalidOperationException("Identity connection string not found.");
    public IDisposable CreateAdapter()
    {
        return new DataAccessAdapter(_connectionString);
    }
}