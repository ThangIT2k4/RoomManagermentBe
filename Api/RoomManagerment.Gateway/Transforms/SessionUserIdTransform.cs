using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace RoomManagerment.Gateway.Transforms;

/// <summary>
/// Đọc UserId từ session và gửi sang Notification API qua header X-User-Id.
/// Chỉ áp dụng cho route "notification".
/// </summary>
public class SessionUserIdTransformProvider(IConfiguration configuration) : ITransformProvider
{
    public const string UserIdHeaderName = "X-User-Id";
    public const string InternalServiceKeyHeaderName = "X-Internal-Service-Key";
    public const string SessionKeyUserId = "UserId";
    public const string NotificationRouteId = "notification";

    public void ValidateRoute(TransformRouteValidationContext context) { }

    public void ValidateCluster(TransformClusterValidationContext context) { }

    public void Apply(TransformBuilderContext context)
    {
        if (!string.Equals(context.Route.RouteId, NotificationRouteId, StringComparison.OrdinalIgnoreCase))
            return;

        var internalSharedKey = configuration["InternalAuth:SharedKey"];

        context.AddRequestTransform(transformContext =>
        {
            var userId = transformContext.HttpContext.Session.GetString(SessionKeyUserId);
            if (!string.IsNullOrEmpty(userId))
                transformContext.ProxyRequest.Headers.TryAddWithoutValidation(UserIdHeaderName, userId);

            if (!string.IsNullOrWhiteSpace(internalSharedKey))
                transformContext.ProxyRequest.Headers.TryAddWithoutValidation(InternalServiceKeyHeaderName, internalSharedKey);

            return ValueTask.CompletedTask;
        });
    }
}
