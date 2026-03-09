using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;

namespace Notification.API.Common;

public static class ControllerBaseExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value!;
        }

        return result.Error?.Code switch
        {
            "Notification.NotFound" or "UserNotification.NotFound" => new NotFoundObjectResult(new { error = result.Error }),
            "UserNotification.Forbidden" => new ObjectResult(new { error = result.Error }) { StatusCode = StatusCodes.Status403Forbidden },
            _ => new BadRequestObjectResult(new { error = result.Error })
        };
    }

    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new NoContentResult();
        }

        return result.Error?.Code switch
        {
            "Notification.NotFound" or "UserNotification.NotFound" => new NotFoundObjectResult(new { error = result.Error }),
            "UserNotification.Forbidden" => new ObjectResult(new { error = result.Error }) { StatusCode = StatusCodes.Status403Forbidden },
            _ => new BadRequestObjectResult(new { error = result.Error })
        };
    }
}
