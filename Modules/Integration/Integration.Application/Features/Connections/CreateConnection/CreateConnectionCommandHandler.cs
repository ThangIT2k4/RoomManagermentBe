using Integration.Application.Common;
using Integration.Application.Dtos;
using Integration.Application.Services;
using Integration.Domain.Entities;
using Integration.Domain.Providers;
using Integration.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Integration.Application.Features.Connections.CreateConnection;

public sealed class CreateConnectionCommandHandler(
    IIntegrationProviderClientFactory providerClientFactory,
    IIntegrationConnectionRepository connectionRepository,
    IIntegrationCredentialRepository credentialRepository,
    ICredentialEncryptionService credentialEncryptionService,
    ILogger<CreateConnectionCommandHandler> logger)
    : IRequestHandler<CreateConnectionCommand, Result<IntegrationConnectionDto>>
{
    public async Task<Result<IntegrationConnectionDto>> Handle(
        CreateConnectionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var providerClient = providerClientFactory.GetClient(request.Provider);
            var providerResult = await providerClient.ConnectAsync(
                new ProviderConnectRequest(
                    request.TenantId,
                    request.UserId,
                    request.AuthorizationCode,
                    request.RedirectUri),
                cancellationToken);

            var connection = IntegrationConnectionEntity.Create(
                request.TenantId,
                request.UserId,
                request.Provider,
                providerResult.ExternalAccountId);

            var credential = IntegrationCredentialEntity.Create(
                connection.Id,
                credentialEncryptionService.Encrypt(providerResult.AccessToken),
                providerResult.RefreshToken is null
                    ? null
                    : credentialEncryptionService.Encrypt(providerResult.RefreshToken),
                providerResult.AccessTokenExpiresAtUtc);

            await connectionRepository.AddAsync(connection, cancellationToken);
            await credentialRepository.AddAsync(credential, cancellationToken);

            logger.LogInformation(
                "Created integration connection {ConnectionId} for provider {Provider}",
                connection.Id,
                request.Provider);

            return Result<IntegrationConnectionDto>.Success(IntegrationConnectionDto.FromEntity(connection));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create integration connection for provider {Provider}", request.Provider);
            return Result<IntegrationConnectionDto>.Failure(
                new Error("INTEGRATION_CONNECT_FAILED", "Unable to create integration connection."));
        }
    }
}
