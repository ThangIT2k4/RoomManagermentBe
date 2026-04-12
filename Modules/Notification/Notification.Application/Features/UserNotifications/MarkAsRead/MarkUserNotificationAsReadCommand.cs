using Notification.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Notification.Application.Features.UserNotifications.MarkAsRead;

public sealed record MarkUserNotificationAsReadCommand(Guid UserNotificationId, Guid UserId) : IAppRequest<Result>;
