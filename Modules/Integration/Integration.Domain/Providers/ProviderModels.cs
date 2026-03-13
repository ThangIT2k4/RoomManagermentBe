namespace Integration.Domain.Providers;

public sealed record ProviderConnectRequest(
    string TenantId,
    string UserId,
    string? AuthorizationCode,
    string? RedirectUri);

public sealed record ProviderConnectResult(
    string ExternalAccountId,
    string AccessToken,
    string? RefreshToken,
    DateTime? AccessTokenExpiresAtUtc);

public sealed record ProviderDisconnectRequest(
    Guid ConnectionId,
    string AccessToken);

public sealed record ProviderActionRequest(
    Guid ConnectionId,
    string ActionName,
    string PayloadJson,
    string AccessToken);

public sealed record ProviderActionResult(
    bool IsSuccess,
    string? ExternalJobId,
    string Message);

public sealed record ProviderWebhookRequest(
    string Signature,
    string PayloadJson,
    IReadOnlyDictionary<string, string> Headers);

public sealed record ProviderWebhookResult(
    bool IsVerified,
    string ExternalEventId,
    string Message);
