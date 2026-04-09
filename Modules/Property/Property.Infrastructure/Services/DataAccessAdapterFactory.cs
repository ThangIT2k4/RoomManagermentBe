using Microsoft.Extensions.Configuration;
using Property.Application.Services;
using RoomManagerment.Property.DatabaseSpecific;

namespace Property.Infrastructure.Services;

public sealed class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = ResolveConnectionString(configuration, "Property", "PROPERTY_CONNECTION_STRING");

    public IDisposable CreateAdapter() => new DataAccessAdapter(_connectionString);

    private static string ResolveConnectionString(IConfiguration configuration, string name, string envAlias)
    {
        var connectionString = configuration.GetConnectionString(name)
            ?? configuration[$"ConnectionStrings:{name}"]
            ?? configuration[envAlias]
            ?? Environment.GetEnvironmentVariable(envAlias);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Missing required connection string '{name}'. Set 'ConnectionStrings__{name}' or '{envAlias}'."
            );
        }

        return connectionString;
    }
}

