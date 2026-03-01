using Identity.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Common;

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
            "User.NotFound" or "Role.NotFound" or "Permission.NotFound" or "Menu.NotFound" or "RefreshToken.NotFound" 
                => new NotFoundObjectResult(new { error = result.Error }),
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
            "User.NotFound" or "Role.NotFound" or "Permission.NotFound" or "Menu.NotFound" or "RefreshToken.NotFound"
                => new NotFoundObjectResult(new { error = result.Error }),
            _ => new BadRequestObjectResult(new { error = result.Error })
        };
    }

    public static ActionResult<T> ToCreatedAtActionResult<T>(
        this Result<T> result,
        string actionName,
        object routeValues)
    {
        if (result.IsSuccess)
        {
            return new CreatedAtActionResult(actionName, null, routeValues, result.Value);
        }

        return result.Error?.Code switch
        {
            "User.NotFound" or "Role.NotFound" or "Permission.NotFound" or "Menu.NotFound"
                => new NotFoundObjectResult(new { error = result.Error }),
            _ => new BadRequestObjectResult(new { error = result.Error })
        };
    }
}
