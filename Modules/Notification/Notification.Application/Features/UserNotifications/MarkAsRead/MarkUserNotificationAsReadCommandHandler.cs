using Notification.Application.Common;
using Notification.Domain.Repositories;
using RoomManagerment.Shared.Messaging;

namespace Notification.Application.Features.UserNotifications.MarkAsRead;

public sealed class MarkUserNotificationAsReadCommandHandler(IUserNotificationRepository userNotificationRepository)
    : IAppRequestHandler<MarkUserNotificationAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkUserNotificationAsReadCommand command, CancellationToken cancellationToken = default)
    {
        var success = await userNotificationRepository.MarkAsReadAsync(command.UserNotificationId, command.UserId, cancellationToken);

        if (!success)
        {
            return Result.Failure(
                new Error("UserNotification.NotFound", $"Không tìm thấy thông báo người dùng với id '{command.UserNotificationId}' hoặc bạn không có quyền truy cập."));
        }

        return Result.Success();
    }
}
