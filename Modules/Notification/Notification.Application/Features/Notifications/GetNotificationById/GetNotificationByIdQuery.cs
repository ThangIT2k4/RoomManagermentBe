using MediatR;
using Notification.Application.Common;

namespace Notification.Application.Features.Notifications.GetNotificationById;

public sealed record GetNotificationByIdQuery(Guid Id) : IRequest<Result<NotificationDto>>;
