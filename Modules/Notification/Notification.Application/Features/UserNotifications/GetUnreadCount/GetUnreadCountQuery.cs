using MediatR;
using Notification.Application.Common;

namespace Notification.Application.Features.UserNotifications.GetUnreadCount;

public sealed record GetUnreadCountQuery(Guid UserId) : IRequest<Result<UnreadCountDto>>;
