using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Common;
using RoomManagerment.Shared.Http;

namespace RoomManagerment.Shared.Extensions;

/// <summary>Maps <see cref="Result{T}"/> / <see cref="Result"/> to HTTP responses with an <see cref="ApiResponse{T}"/> body.</summary>
public static class ApiResultHttpExtensions
{
    public static ActionResult<ApiResponse<T>> ToApiActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(ApiResponse<T>.Succeed(result.Value!));
        }

        return controller.ToApiFailureObjectResult<T>(result.Error);
    }

    /// <summary>Maps a successful <see cref="Result"/> to 200 OK with <see cref="ApiResponse{TPayload}"/> and <c>data: new TPayload()</c>.</summary>
    public static ActionResult<ApiResponse<TPayload>> ToApiVoidActionResult<TPayload>(this ControllerBase controller, Result result)
        where TPayload : new()
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(ApiResponse<TPayload>.Succeed(new TPayload()));
        }

        return controller.ToApiFailureObjectResult<TPayload>(result.Error);
    }

    public static ActionResult<ApiResponse<T>> ToApiCreatedAtActionResult<T>(
        this ControllerBase controller,
        Result<T> result,
        string actionName,
        object routeValues)
    {
        if (result.IsSuccess)
        {
            return new CreatedAtActionResult(
                actionName,
                null,
                routeValues,
                ApiResponse<T>.Succeed(result.Value!));
        }

        return controller.ToApiFailureObjectResult<T>(result.Error);
    }

    public static ActionResult<ApiResponse<T>> ApiUnauthorized<T>(this ControllerBase _, string message) =>
        new ObjectResult(ApiResponse<T>.Failure(message))
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

    public static ActionResult<ApiResponse<T>> ApiBadRequest<T>(this ControllerBase _, string message) =>
        new ObjectResult(ApiResponse<T>.Failure(message))
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

    public static ObjectResult ApiServerError<T>(this ControllerBase _, string message) =>
        new ObjectResult(ApiResponse<T>.Failure(message))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

    /// <summary>Maps an <see cref="Error"/> to <see cref="ApiResponse{T}"/> with the appropriate status code (e.g. dashboard built from another <see cref="Result"/>).</summary>
    public static ActionResult<ApiResponse<T>> ToApiFailureResult<T>(this ControllerBase controller, Error? error) =>
        controller.ToApiFailureObjectResult<T>(error);

    public static ActionResult<ApiResponse<T>> ApiNotFound<T>(this ControllerBase _, string message) =>
        new ObjectResult(ApiResponse<T>.Failure(message))
        {
            StatusCode = StatusCodes.Status404NotFound
        };

    public static ActionResult<ApiResponse<T>> ApiNotImplemented<T>(this ControllerBase _, string message) =>
        new ObjectResult(ApiResponse<T>.Failure(message))
        {
            StatusCode = StatusCodes.Status501NotImplemented
        };

    private static ActionResult<ApiResponse<T>> ToApiFailureObjectResult<T>(this ControllerBase controller, Error? error)
    {
        var message = FormatErrorMessage(error);
        var body = ApiResponse<T>.Failure(message);
        return new ObjectResult(body) { StatusCode = StatusCodeFor(error?.Type ?? ErrorType.Unexpected) };
    }

    private static string FormatErrorMessage(Error? error)
    {
        if (error is null)
        {
            return "Đã xảy ra lỗi không xác định.";
        }

        if (error.FieldErrors is { Count: > 0 } fields)
        {
            return string.Join(" ", fields.Select(f => f.Message));
        }

        return error.Message;
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
