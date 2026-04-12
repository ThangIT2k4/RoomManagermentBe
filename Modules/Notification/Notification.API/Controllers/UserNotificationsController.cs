using Microsoft.AspNetCore.Mvc;
using Notification.API.Common;
using Notification.Application.Common;
using Notification.Application.Dtos;
using Notification.Application.Features.UserNotifications.GetUnreadCount;
using Notification.Application.Features.UserNotifications.GetUserNotificationsPaged;
using Notification.Application.Features.UserNotifications.MarkAsRead;
using Notification.Application.Services;
using RoomManagerment.Shared.Extensions;
using RoomManagerment.Shared.Http;

namespace Notification.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserNotificationsController(IMediatorGateway mediator) : ControllerBase
{
    private bool TryGetUserIdFromSessionHeader(out Guid userId)
    {
        var value = Request.Headers[SessionUserIdHeader.Name].FirstOrDefault();
        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out userId))
        {
            userId = default;
            return false;
        }
        return true;
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<UnreadCountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UnreadCountDto>>> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        if (!TryGetUserIdFromSessionHeader(out var userId))
        {
            return this.ApiUnauthorized<UnreadCountDto>("Session required. Call via Gateway with cookie.");
        }

        var query = new GetUnreadCountQuery(userId);
        var result = await mediator.SendAsync<Result<UnreadCountDto>>(query, cancellationToken);
        return this.ToNotificationApiActionResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserNotificationListItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResponse<UserNotificationListItemDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isRead = null,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetUserIdFromSessionHeader(out var userId))
        {
            return this.ApiUnauthorized<PagedResponse<UserNotificationListItemDto>>("Session required. Call via Gateway with cookie.");
        }

        var query = new GetUserNotificationsPagedQuery(userId, page, pageSize, isRead);
        var result = await mediator.SendAsync<Result<PagedResponse<UserNotificationListItemDto>>>(query, cancellationToken);
        return this.ToNotificationApiActionResult(result);
    }

    [HttpPost("{userNotificationId}/mark-read")]
    [ProducesResponseType(typeof(ApiResponse<NotificationMarkAsReadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<NotificationMarkAsReadResponse>>> MarkAsRead(
        [FromRoute] Guid userNotificationId,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetUserIdFromSessionHeader(out var userId))
        {
            return this.ApiUnauthorized<NotificationMarkAsReadResponse>("Session required. Call via Gateway with cookie.");
        }

        var command = new MarkUserNotificationAsReadCommand(userNotificationId, userId);
        var result = await mediator.SendAsync<Result>(command, cancellationToken);
        return this.ToNotificationApiVoidResult<NotificationMarkAsReadResponse>(result);
    }
}
