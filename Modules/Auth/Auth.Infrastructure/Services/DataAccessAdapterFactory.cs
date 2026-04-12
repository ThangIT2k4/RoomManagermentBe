using Auth.Application.Services;
using Microsoft.Extensions.Configuration;
using RoomManagerment.Auth.DatabaseSpecific;

namespace Auth.Infrastructure.Services;

public sealed class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = ResolveConnectionString(configuration, "Auth", "AUTH_CONNECTION_STRING");

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
                $"Thiếu chuỗi kết nối bắt buộc '{name}'. Hãy cấu hình 'ConnectionStrings__{name}' hoặc '{envAlias}'."
            );
        }

        return connectionString;
    }
}

