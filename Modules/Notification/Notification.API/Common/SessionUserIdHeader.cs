namespace Notification.API.Common;

/// <summary>
/// Header do Gateway gửi sang (từ session) khi proxy request tới Notification.
/// Không nhận userId qua query/body; chỉ dùng header này.
/// </summary>
public static class SessionUserIdHeader
{
    public const string Name = "X-User-Id";
}
