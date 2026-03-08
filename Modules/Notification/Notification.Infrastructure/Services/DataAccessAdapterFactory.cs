using Microsoft.Extensions.Configuration;
using Notification.Application.Services;
using RoomManagerment.Notification.DatabaseSpecific;

namespace Notification.Infrastructure.Services;

public class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("Notification")
        ?? throw new InvalidOperationException("Notification connection string not found.");

    public IDisposable CreateAdapter()
    {
        return new DataAccessAdapter(_connectionString);
    }
}
