using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomManagerment.Messaging.Configuration;
using System.Globalization;

namespace RoomManagerment.Messaging.Extensions;

public static class MessagingServiceExtensions
{
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null
    )
    {
        var options = ResolveRabbitMqOptions(configuration);

        services
            .AddOptions<RabbitMqOptions>()
            .Configure(o =>
            {
                o.Host = options.Host;
                o.Port = options.Port;
                o.Username = options.Username;
                o.Password = options.Password;
                o.VirtualHost = options.VirtualHost;
                o.RetryCount = options.RetryCount;
                o.RetryIntervalMs = options.RetryIntervalMs;
            })
            .ValidateDataAnnotations()
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "RabbitMq:Host is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Username), "RabbitMq:Username is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "RabbitMq:Password is required")
            .ValidateOnStart();


        services.AddMassTransit(x =>
        {
            configureConsumers?.Invoke(x);
            
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(options.Host, (ushort)options.Port, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });
                
                cfg.UseMessageRetry(r => r.Incremental(
                    options.RetryCount,
                    TimeSpan.FromMilliseconds(options.RetryIntervalMs),
                    TimeSpan.FromMilliseconds(options.RetryIntervalMs)
                ));
                
                cfg.ConfigureEndpoints(ctx);
            });
        });
        
        return services;
    }

    private static RabbitMqOptions ResolveRabbitMqOptions(IConfiguration configuration)
    {
        var sectionOptions = configuration.GetSection(RabbitMqOptions.Section).Get<RabbitMqOptions>();
        var envFileValues = LoadEnvLikeFiles();

        var host = FirstNonEmpty(
            sectionOptions?.Host,
            configuration[$"{RabbitMqOptions.Section}:Host"],
            configuration["RABBITMQ_HOST"],
            GetEnvValue(envFileValues, "RABBITMQ_HOST")
        );

        host = NormalizeRabbitMqHostForLocalRun(host);

        var username = FirstNonEmpty(
            sectionOptions?.Username,
            configuration[$"{RabbitMqOptions.Section}:Username"],
            configuration["RABBITMQ_USER"],
            GetEnvValue(envFileValues, "RABBITMQ_USER"),
            configuration["RABBITMQ_DEFAULT_USER"],
            GetEnvValue(envFileValues, "RABBITMQ_DEFAULT_USER")
        );

        var password = FirstNonEmpty(
            sectionOptions?.Password,
            configuration[$"{RabbitMqOptions.Section}:Password"],
            configuration["RABBITMQ_PASS"],
            GetEnvValue(envFileValues, "RABBITMQ_PASS"),
            configuration["RABBITMQ_DEFAULT_PASS"],
            GetEnvValue(envFileValues, "RABBITMQ_DEFAULT_PASS")
        );

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Missing RabbitMQ configuration. Provide section '{RabbitMqOptions.Section}' or env vars " +
                "RABBITMQ_HOST with RABBITMQ_USER and RABBITMQ_PASS (or legacy RABBITMQ_DEFAULT_USER / RABBITMQ_DEFAULT_PASS)."
            );
        }

        return new RabbitMqOptions
        {
            Host = host,
            Username = username,
            Password = password,
            Port = ParseInt(
                sectionOptions?.Port,
                configuration[$"{RabbitMqOptions.Section}:Port"],
                configuration["RABBITMQ_PORT"],
                GetEnvValue(envFileValues, "RABBITMQ_PORT"),
                5672
            ),
            VirtualHost = FirstNonEmpty(
                sectionOptions?.VirtualHost,
                configuration[$"{RabbitMqOptions.Section}:VirtualHost"],
                configuration["RABBITMQ_VHOST"],
                GetEnvValue(envFileValues, "RABBITMQ_VHOST")
            ) ?? "/",
            RetryCount = ParseInt(
                sectionOptions?.RetryCount,
                configuration[$"{RabbitMqOptions.Section}:RetryCount"],
                null,
                null,
                3
            ),
            RetryIntervalMs = ParseInt(
                sectionOptions?.RetryIntervalMs,
                configuration[$"{RabbitMqOptions.Section}:RetryIntervalMs"],
                null,
                null,
                1000
            )
        };
    }

    private static Dictionary<string, string> LoadEnvLikeFiles()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var cwd = Directory.GetCurrentDirectory();

        foreach (var path in EnumerateCandidateEnvFiles(cwd))
        {
            if (!File.Exists(path))
            {
                continue;
            }

            foreach (var line in File.ReadLines(path))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
                {
                    continue;
                }

                var separatorIndex = trimmed.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = trimmed[..separatorIndex].Trim();
                var value = trimmed[(separatorIndex + 1)..].Trim();

                if (value.Length >= 2 && value.StartsWith('"') && value.EndsWith('"'))
                {
                    value = value[1..^1];
                }

                result[key] = value;
            }
        }

        return result;
    }

    private static IEnumerable<string> EnumerateCandidateEnvFiles(string startDirectory)
    {
        var current = new DirectoryInfo(startDirectory);
        while (current is not null)
        {
            yield return Path.Combine(current.FullName, ".env.local");
            yield return Path.Combine(current.FullName, ".env");
            current = current.Parent;
        }
    }

    private static string? GetEnvValue(IReadOnlyDictionary<string, string> values, string key)
        => values.TryGetValue(key, out var value) ? value : null;

    private static string? FirstNonEmpty(params string?[] candidates)
        => candidates.FirstOrDefault(static c => !string.IsNullOrWhiteSpace(c));

    private static int ParseInt(int? sectionValue, string? configValue, string? envValue, string? envFileValue, int fallback)
    {
        if (sectionValue.HasValue && sectionValue.Value > 0)
        {
            return sectionValue.Value;
        }

        foreach (var candidate in new[] { configValue, envValue, envFileValue })
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                continue;
            }

            if (int.TryParse(candidate, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) && parsed > 0)
            {
                return parsed;
            }
        }

        return fallback;
    }

    private static bool IsRunningInContainer()
        => string.Equals(
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
            "true",
            StringComparison.OrdinalIgnoreCase
        );

    /// <summary>
    /// Compose DNS name is <c>rabbitmq</c>. Legacy env files used wrong hostnames.
    /// From the host OS (<c>dotnet run</c>), those names do not resolve; use published port on localhost.
    /// </summary>
    private static string? NormalizeRabbitMqHostForLocalRun(string? host)
    {
        if (string.IsNullOrWhiteSpace(host))
            return host;

        if (IsRunningInContainer())
            return host;

        if (string.Equals(host, "rabbitmq", StringComparison.OrdinalIgnoreCase)
            || string.Equals(host, "room_managerment_rabbitmq", StringComparison.OrdinalIgnoreCase)
            || string.Equals(host, "rm-rabbitmq", StringComparison.OrdinalIgnoreCase))
        {
            return "localhost";
        }

        return host;
    }
}
