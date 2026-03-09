using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notification.API.Common;
using Notification.Application.Common;
using Notification.Application.Features.UserNotifications.GetUnreadCount;
using Notification.Application.Features.UserNotifications.GetUserNotificationsPaged;
using Notification.Application.Features.UserNotifications.MarkAsRead;

namespace Notification.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserNotificationsController(IMediator mediator) : ControllerBase
{
    /// <summary>Lấy userId từ header X-User-Id (Gateway set từ session). Trả về 401 nếu thiếu/không hợp lệ.</summary>
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
    [ProducesResponseType(typeof(UnreadCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UnreadCountDto>> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        if (!TryGetUserIdFromSessionHeader(out var userId))
            return Unauthorized(new { error = "Session required. Call via Gateway with cookie." });

        var query = new GetUnreadCountQuery(userId);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<UserNotificationListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<UserNotificationListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isRead = null,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetUserIdFromSessionHeader(out var userId))
            return Unauthorized(new { error = "Session required. Call via Gateway with cookie." });

        var query = new GetUserNotificationsPagedQuery(userId, page, pageSize, isRead);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{userNotificationId}/mark-read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(
        [FromRoute] Guid userNotificationId,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetUserIdFromSessionHeader(out var userId))
            return Unauthorized(new { error = "Session required. Call via Gateway with cookie." });

        var command = new MarkUserNotificationAsReadCommand(userNotificationId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
