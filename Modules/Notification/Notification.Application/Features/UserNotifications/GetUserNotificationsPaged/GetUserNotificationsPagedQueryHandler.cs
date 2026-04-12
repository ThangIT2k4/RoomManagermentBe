using Notification.Application.Common;
using Notification.Domain.Repositories;
using RoomManagerment.Shared.Messaging;

namespace Notification.Application.Features.UserNotifications.GetUserNotificationsPaged;

public sealed class GetUserNotificationsPagedQueryHandler(IUserNotificationRepository userNotificationRepository)
    : IAppRequestHandler<GetUserNotificationsPagedQuery, Result<PagedResponse<UserNotificationListItemDto>>>
{
    public async Task<Result<PagedResponse<UserNotificationListItemDto>>> Handle(
        GetUserNotificationsPagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var paged = await userNotificationRepository.GetByUserIdPagedAsync(
            query.UserId, query.Page, query.PageSize, query.IsRead, cancellationToken);

        var items = paged.Items
            .Select(un => new UserNotificationListItemDto(
                un.Id,
                un.UserId,
                un.Id,
                un.Title,
                un.Content,
                un.Type,
                un.IsRead,
                un.ReadAt,
                un.CreatedAt))
            .ToList();

        var response = new PagedResponse<UserNotificationListItemDto>(
            items,
            paged.TotalCount,
            paged.Page,
            paged.PageSize,
            paged.TotalPages,
            paged.HasPreviousPage,
            paged.HasNextPage);

        return Result<PagedResponse<UserNotificationListItemDto>>.Success(response);
    }
}
