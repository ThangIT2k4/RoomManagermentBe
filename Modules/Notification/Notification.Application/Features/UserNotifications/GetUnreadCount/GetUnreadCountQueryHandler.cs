using MediatR;
using Notification.Application.Common;
using Notification.Domain.Repositories;

namespace Notification.Application.Features.UserNotifications.GetUnreadCount;

public sealed class GetUnreadCountQueryHandler(IUserNotificationRepository userNotificationRepository)
    : IRequestHandler<GetUnreadCountQuery, Result<UnreadCountDto>>
{
    public async Task<Result<UnreadCountDto>> Handle(GetUnreadCountQuery query, CancellationToken cancellationToken = default)
    {
        var count = await userNotificationRepository.GetUnreadCountAsync(query.UserId, cancellationToken);
        return Result<UnreadCountDto>.Success(new UnreadCountDto(count));
    }
}
