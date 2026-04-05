namespace Notification.Application.Services;

/// <summary>
/// Tạo user notification từ integration events (Auth, Finance, …). Logic chung tập trung ở Application.
/// </summary>
public interface IUserNotificationIngestionService
{
    Task<Guid> CreateAsync(
        Guid userId,
        string title,
        string content,
        string notificationType,
        DateTime? createdAtUtc,
        CancellationToken cancellationToken = default);
}
