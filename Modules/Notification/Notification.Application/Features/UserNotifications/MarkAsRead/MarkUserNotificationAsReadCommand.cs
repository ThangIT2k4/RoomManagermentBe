using MediatR;
using Notification.Application.Common;

namespace Notification.Application.Features.UserNotifications.MarkAsRead;

public sealed record MarkUserNotificationAsReadCommand(Guid UserNotificationId, Guid UserId) : IRequest<Result>;
