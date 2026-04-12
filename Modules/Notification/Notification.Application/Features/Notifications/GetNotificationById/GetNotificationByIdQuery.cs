using Notification.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Notification.Application.Features.Notifications.GetNotificationById;

public sealed record GetNotificationByIdQuery(Guid Id) : IAppRequest<Result<NotificationDto>>;
