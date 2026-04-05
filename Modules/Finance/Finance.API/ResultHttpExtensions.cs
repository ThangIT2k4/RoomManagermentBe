using Finance.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API;

internal static class ResultHttpExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkResult();
        }

        return MapFailureToIActionResult(result.Error);
    }

    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value!;
        }

        return MapFailureToActionResult<T>(result.Error);
    }

    private static IActionResult MapFailureToIActionResult(Error? error)
    {
        if (error is null)
        {
            return new BadRequestResult();
        }

        if (error.Code.Contains("NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return new NotFoundObjectResult(new { code = error.Code, message = error.Message });
        }

        if (error.Code.Contains("Forbidden", StringComparison.OrdinalIgnoreCase))
        {
            return new ObjectResult(new { code = error.Code, message = error.Message })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return new BadRequestObjectResult(new { code = error.Code, message = error.Message });
    }

    private static ActionResult<T> MapFailureToActionResult<T>(Error? error)
    {
        if (error is null)
        {
            return new BadRequestResult();
        }

        if (error.Code.Contains("NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return new NotFoundObjectResult(new { code = error.Code, message = error.Message });
        }

        if (error.Code.Contains("Forbidden", StringComparison.OrdinalIgnoreCase))
        {
            return new ObjectResult(new { code = error.Code, message = error.Message })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return new BadRequestObjectResult(new { code = error.Code, message = error.Message });
    }
}
