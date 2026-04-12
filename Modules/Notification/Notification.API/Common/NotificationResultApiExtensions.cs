using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;
using RoomManagerment.Shared.Http;

namespace Notification.API.Common;

/// <summary>Maps Notification module <see cref="Result"/> / <see cref="Result{T}"/> to <see cref="ApiResponse{T}"/> (status rules match legacy <see cref="ControllerBaseExtensions"/>).</summary>
public static class NotificationResultApiExtensions
{
    public static ActionResult<ApiResponse<T>> ToNotificationApiActionResult<T>(this ControllerBase _, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(ApiResponse<T>.Succeed(result.Value!));
        }

        return ToFailureResult<T>(result.Error);
    }

    public static ActionResult<ApiResponse<TPayload>> ToNotificationApiVoidResult<TPayload>(this ControllerBase _, Result result)
        where TPayload : new()
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(ApiResponse<TPayload>.Succeed(new TPayload()));
        }

        return ToFailureResult<TPayload>(result.Error);
    }

    private static ActionResult<ApiResponse<T>> ToFailureResult<T>(Error? error)
    {
        var message = error?.Message ?? "Request failed.";
        var body = ApiResponse<T>.Failure(message);
        var statusCode = error?.Code switch
        {
            "Notification.NotFound" or "UserNotification.NotFound" => StatusCodes.Status404NotFound,
            "UserNotification.Forbidden" => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        return new ObjectResult(body) { StatusCode = statusCode };
    }
}
