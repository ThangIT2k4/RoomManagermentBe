using Integration.Domain.Entities;

namespace Integration.Domain.Tests.Entities;

public sealed class IntegrationCredentialEntityTests
{
    [Fact]
    public void UpdateTokens_ShouldUpdateCredentialValues()
    {
        var entity = IntegrationCredentialEntity.Create(
            Guid.NewGuid(),
            "token-v1",
            "refresh-v1",
            DateTime.UtcNow.AddHours(1));

        entity.UpdateTokens("token-v2", "refresh-v2", DateTime.UtcNow.AddHours(2));

        Assert.Equal("token-v2", entity.EncryptedAccessToken);
        Assert.Equal("refresh-v2", entity.EncryptedRefreshToken);
        Assert.NotNull(entity.AccessTokenExpiresAtUtc);
    }

    [Fact]
    public void Clear_ShouldRemoveTokenData()
    {
        var entity = IntegrationCredentialEntity.Create(
            Guid.NewGuid(),
            "token-v1",
            "refresh-v1",
            DateTime.UtcNow.AddHours(1));

        entity.Clear();

        Assert.Equal(string.Empty, entity.EncryptedAccessToken);
        Assert.Null(entity.EncryptedRefreshToken);
        Assert.Null(entity.AccessTokenExpiresAtUtc);
    }
}

