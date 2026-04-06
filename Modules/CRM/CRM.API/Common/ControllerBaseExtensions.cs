using CRM.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Common;

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

    private static ActionResult ToActionResult(this Error? error)
    {
        if (error is null)
        {
            return new BadRequestObjectResult(new { error = new Error("CRM.Unknown", "Unknown error.") });
        }

        if (error.Code.Contains("NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return new NotFoundObjectResult(new { error });
        }

        return new BadRequestObjectResult(new { error });
    }
}
