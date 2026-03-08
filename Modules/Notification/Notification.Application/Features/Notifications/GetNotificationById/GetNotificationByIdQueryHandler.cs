using MediatR;
using Notification.Application.Common;
using Notification.Domain.Repositories;

namespace Notification.Application.Features.Notifications.GetNotificationById;

public sealed class GetNotificationByIdQueryHandler(INotificationRepository notificationRepository)
    : IRequestHandler<GetNotificationByIdQuery, Result<NotificationDto>>
{
    public async Task<Result<NotificationDto>> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var notification = await notificationRepository.GetByIdAsync(query.Id, cancellationToken);

        if (notification is null)
        {
            return Result<NotificationDto>.Failure(
                new Error("Notification.NotFound", $"Notification with id '{query.Id}' was not found."));
        }

        var dto = new NotificationDto(
            notification.Id,
            notification.Title,
            notification.Content,
            notification.Type,
            notification.CreatedAt);

        return Result<NotificationDto>.Success(dto);
    }
}
