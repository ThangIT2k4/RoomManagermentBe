using Microsoft.Extensions.Configuration;

namespace RoomManagerment.Messaging.Extensions;

/// <summary>
/// Resolves the Redis connection string using the same priority chain and .env file
/// fallback as <see cref="MessagingServiceExtensions"/> uses for RabbitMQ.
///
/// Priority order:
///   1. Redis:Configuration  (env Redis__Configuration  — compose standard)
///   2. Redis:ConnectionString (alternative section key)
///   3. ConnectionStrings:Redis (env ConnectionStrings__Redis)
///   4. REDIS_CONFIGURATION flat env var (process environment)
///   5. REDIS_CONFIGURATION key in .env.local / .env file (walk-up)
///   6. Smart default: "redis:6379" in container, "localhost:6379" outside
/// </summary>
public static class RedisServiceExtensions
{
    public static string ResolveConnectionString(IConfiguration configuration)
    {
        var fromSection = configuration["Redis:Configuration"];
        if (!string.IsNullOrWhiteSpace(fromSection))
            return NormalizeHost(fromSection);

        var fromAltSection = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(fromAltSection))
            return NormalizeHost(fromAltSection);

        var fromConnectionStrings = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(fromConnectionStrings))
            return NormalizeHost(fromConnectionStrings);

        var fromProcessEnv = Environment.GetEnvironmentVariable("REDIS_CONFIGURATION");
        if (!string.IsNullOrWhiteSpace(fromProcessEnv))
            return NormalizeHost(fromProcessEnv);

        var envFileValues = LoadEnvLikeFiles();
        var fromFile = GetEnvValue(envFileValues, "REDIS_CONFIGURATION");
        if (!string.IsNullOrWhiteSpace(fromFile))
            return NormalizeHost(fromFile);

        return IsRunningInContainer() ? "redis:6379" : "localhost:6379";
    }

    /// <summary>
    /// Replaces the "redis" Docker-service hostname with "localhost" when
    /// running outside a container, mirroring the RabbitMQ host normalisation.
    /// </summary>
    private static string NormalizeHost(string connectionString)
    {
        if (IsRunningInContainer())
            return connectionString;

        // Connection string starts with "redis:PORT" or "redis,..." — swap hostname only
        if (connectionString.StartsWith("redis:", StringComparison.OrdinalIgnoreCase)
            || connectionString.StartsWith("redis,", StringComparison.OrdinalIgnoreCase))
        {
            return "localhost" + connectionString["redis".Length..];
        }

        return connectionString;
    }

    private static Dictionary<string, string> LoadEnvLikeFiles()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (current is not null)
        {
            foreach (var fileName in new[] { ".env.local", ".env" })
            {
                var path = Path.Combine(current.FullName, fileName);
                if (!File.Exists(path))
                    continue;

                foreach (var line in File.ReadLines(path))
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
                        continue;

                    var sep = trimmed.IndexOf('=');
                    if (sep <= 0)
                        continue;

                    var key = trimmed[..sep].Trim();
                    var value = trimmed[(sep + 1)..].Trim();

                    if (value.Length >= 2 && value.StartsWith('"') && value.EndsWith('"'))
                        value = value[1..^1];

                    result.TryAdd(key, value);
                }
            }

            current = current.Parent;
        }

        return result;
    }

    private static string? GetEnvValue(IReadOnlyDictionary<string, string> values, string key)
        => values.TryGetValue(key, out var value) ? value : null;

    private static bool IsRunningInContainer()
        => string.Equals(
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
            "true",
            StringComparison.OrdinalIgnoreCase);
}
