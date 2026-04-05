using Auth.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Common;

public static class ControllerBaseExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value!;
        }

        return result.Error.ToActionResult();
    }

    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new NoContentResult();
        }

        return result.Error.ToActionResult();
    }

    public static ActionResult<T> ToCreatedAtActionResult<T>(this Result<T> result, string actionName, object routeValues)
    {
        if (result.IsSuccess)
        {
            return new CreatedAtActionResult(actionName, null, routeValues, result.Value);
        }

        return result.Error.ToActionResult();
    }

    private static ActionResult ToActionResult(this Error? error)
    {
        if (error is null)
        {
            return new BadRequestObjectResult(new { error = new Error("Auth.Unknown", "Unknown error.") });
        }

        if (error.Code.Contains("NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return new NotFoundObjectResult(new { error });
        }

        if (error.Code.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) ||
            error.Code.Equals("Auth.Login.Failed", StringComparison.OrdinalIgnoreCase) ||
            error.Code.Equals("Auth.Session.NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return new UnauthorizedObjectResult(new { error });
        }

        if (error.Code.Equals("Auth.Login.NotVerified", StringComparison.OrdinalIgnoreCase) ||
            error.Code.Equals("Auth.Login.Banned", StringComparison.OrdinalIgnoreCase))
        {
            return new ObjectResult(new { error }) { StatusCode = StatusCodes.Status403Forbidden };
        }

        if (error.Code.Contains("Exists", StringComparison.OrdinalIgnoreCase))
        {
            return new ConflictObjectResult(new { error });
        }

        return new BadRequestObjectResult(new { error });
    }
}
