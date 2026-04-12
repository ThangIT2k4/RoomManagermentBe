using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Common;
using RoomManagerment.Shared.Http;

namespace RoomManagerment.Shared.Extensions;

public static class ResultHttpExtensions
{
    /// <summary>Maps <see cref="Result{T}"/> using the controller's <see cref="HttpContext"/> (e.g. correlation id).</summary>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller) =>
        controller.ToActionResult(result);

    /// <summary>Maps non-generic <see cref="Result"/> using the controller's <see cref="HttpContext"/>.</summary>
    public static ActionResult ToActionResult(this Result result, ControllerBase controller) =>
        controller.ToActionResult(result);

    public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value!;
        }

        return controller.ToFailureObjectResult<T>(result.Error);
    }

    public static ActionResult ToActionResult(this ControllerBase controller, Result result)
    {
        if (result.IsSuccess)
        {
            return new NoContentResult();
        }

        return controller.ToFailureObjectResult(result.Error);
    }

    public static ActionResult<T> ToCreatedAtActionResult<T>(
        this ControllerBase controller,
        Result<T> result,
        string actionName,
        object routeValues)
    {
        if (result.IsSuccess)
        {
            return new CreatedAtActionResult(actionName, null, routeValues, result.Value);
        }

        return controller.ToFailureObjectResult<T>(result.Error);
    }

    private static ActionResult<T> ToFailureObjectResult<T>(this ControllerBase controller, Error? error)
    {
        var problem = BuildProblemDetails(controller.HttpContext, error);
        return new ObjectResult(problem) { StatusCode = StatusCodeFor(error?.Type ?? ErrorType.Unexpected) };
    }

    private static ActionResult ToFailureObjectResult(this ControllerBase controller, Error? error)
    {
        var problem = BuildProblemDetails(controller.HttpContext, error);
        return new ObjectResult(problem) { StatusCode = StatusCodeFor(error?.Type ?? ErrorType.Unexpected) };
    }

    private static ProblemDetails BuildProblemDetails(HttpContext httpContext, Error? error)
    {
        var correlationId = httpContext.Items[CorrelationIdConstants.HttpContextItemKey]?.ToString() ?? string.Empty;

        if (error is null)
        {
            return new ProblemDetails
            {
                Title = "Unknown error",
                Detail = "An unknown error occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path.Value,
                Extensions =
                {
                    ["code"] = "Unknown",
                    ["traceId"] = correlationId
                }
            };
        }

        var problem = new ProblemDetails
        {
            Title = error.Message,
            Detail = error.Message,
            Status = StatusCodeFor(error.Type),
            Instance = httpContext.Request.Path.Value,
            Extensions =
            {
                ["code"] = error.Code,
                ["traceId"] = correlationId
            }
        };

        if (error.FieldErrors is { Count: > 0 } fields)
        {
            problem.Extensions["errors"] = fields
                .Select(f => new { field = f.Field, message = f.Message })
                .ToList();
        }

        return problem;
    }

    private static int StatusCodeFor(ErrorType type) =>
        type switch
        {
            ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.BadRequest => StatusCodes.Status400BadRequest,
            ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
}
