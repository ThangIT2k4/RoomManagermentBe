using Notification.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Notification.Application.Features.UserNotifications.GetUnreadCount;

public sealed record GetUnreadCountQuery(Guid UserId) : IAppRequest<Result<UnreadCountDto>>;
