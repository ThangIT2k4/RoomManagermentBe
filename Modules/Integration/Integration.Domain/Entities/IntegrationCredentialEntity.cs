namespace Integration.Domain.Entities;

public sealed class IntegrationCredentialEntity
{
    public Guid ConnectionId { get; private set; }
    public string EncryptedAccessToken { get; private set; }
    public string? EncryptedRefreshToken { get; private set; }
    public DateTime? AccessTokenExpiresAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private IntegrationCredentialEntity()
    {
        EncryptedAccessToken = string.Empty;
    }

    private IntegrationCredentialEntity(
        Guid connectionId,
        string encryptedAccessToken,
        string? encryptedRefreshToken,
        DateTime? accessTokenExpiresAtUtc)
    {
        ConnectionId = connectionId;
        EncryptedAccessToken = encryptedAccessToken;
        EncryptedRefreshToken = encryptedRefreshToken;
        AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public static IntegrationCredentialEntity Create(
        Guid connectionId,
        string encryptedAccessToken,
        string? encryptedRefreshToken,
        DateTime? accessTokenExpiresAtUtc)
    {
        return new IntegrationCredentialEntity(connectionId, encryptedAccessToken, encryptedRefreshToken, accessTokenExpiresAtUtc);
    }

    public void UpdateTokens(string encryptedAccessToken, string? encryptedRefreshToken, DateTime? accessTokenExpiresAtUtc)
    {
        EncryptedAccessToken = encryptedAccessToken;
        EncryptedRefreshToken = encryptedRefreshToken;
        AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Clear()
    {
        EncryptedAccessToken = string.Empty;
        EncryptedRefreshToken = null;
        AccessTokenExpiresAtUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
