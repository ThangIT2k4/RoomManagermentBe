using Microsoft.AspNetCore.Http;

namespace Auth.API.Common;

public static class HttpRequestExtensions
{
    /// <summary>
    /// Best-effort client IP after UseForwardedHeaders (RemoteIpAddress is rewritten when configured).
    /// </summary>
    public static string? GetClientIpAddress(this HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString();
    }

    public static string? GetClientUserAgent(this HttpContext context)
    {
        var ua = context.Request.Headers.UserAgent.ToString();
        return string.IsNullOrWhiteSpace(ua) ? null : ua;
    }
}
