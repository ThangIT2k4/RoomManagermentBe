using Notification.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Notification.Application.Features.UserNotifications.GetUserNotificationsPaged;

public sealed record GetUserNotificationsPagedQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 10,
    bool? IsRead = null) : IAppRequest<Result<PagedResponse<UserNotificationListItemDto>>>;
