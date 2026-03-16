using System.ComponentModel.DataAnnotations;

namespace RoomManagerment.Messaging.Configuration;

public sealed class RabbitMqOptions
{
    public const string Section = "RabbitMq";

    [Required(ErrorMessage = "RabbitMq:Host is required")]
    public string Host { get; set; } = default!;

    [Range(1, 65535)]
    public int Port { get; set; } = 5672;

    [Required(ErrorMessage = "RabbitMq:Username is required")]
    public string Username { get; set; } = default!;

    [Required(ErrorMessage = "RabbitMq:Password is required")]
    public string Password { get; set; } = default!;

    public string VirtualHost { get; set; } = "/";

    [Range(0, 20)]
    public int RetryCount { get; set; } = 3;

    [Range(100, 60000)]
    public int RetryIntervalMs { get; set; } = 1000;
}
