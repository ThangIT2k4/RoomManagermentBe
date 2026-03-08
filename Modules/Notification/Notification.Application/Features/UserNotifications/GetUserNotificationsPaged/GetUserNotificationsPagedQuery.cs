using MediatR;
using Notification.Application.Common;

namespace Notification.Application.Features.UserNotifications.GetUserNotificationsPaged;

public sealed record GetUserNotificationsPagedQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 10,
    bool? IsRead = null) : IRequest<Result<PagedResponse<UserNotificationListItemDto>>>;
