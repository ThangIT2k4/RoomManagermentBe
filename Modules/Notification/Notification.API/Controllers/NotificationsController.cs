using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notification.API.Common;
using Notification.Application.Features.Notifications.GetNotificationById;

namespace Notification.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetNotificationByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
