using Integration.Application.Common;
using Integration.Application.Services;
using Integration.Domain.Providers;
using Integration.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Integration.Application.Features.Connections.DisconnectConnection;

public sealed class DisconnectConnectionCommandHandler(
    IIntegrationConnectionRepository connectionRepository,
    IIntegrationCredentialRepository credentialRepository,
    IIntegrationProviderClientFactory providerClientFactory,
    ICredentialEncryptionService credentialEncryptionService,
    ILogger<DisconnectConnectionCommandHandler> logger)
    : IRequestHandler<DisconnectConnectionCommand, Result>
{
    public async Task<Result> Handle(DisconnectConnectionCommand request, CancellationToken cancellationToken)
    {
        var connection = await connectionRepository.GetByIdAsync(request.ConnectionId, cancellationToken);
        if (connection is null)
        {
            return Result.Failure(new Error("INTEGRATION_NOT_FOUND", "Connection not found."));
        }

        var credential = await credentialRepository.GetByConnectionIdAsync(request.ConnectionId, cancellationToken);
        if (credential is null)
        {
            return Result.Failure(new Error("CREDENTIAL_NOT_FOUND", "Credential not found."));
        }

        try
        {
            var providerClient = providerClientFactory.GetClient(connection.Provider);
            var accessToken = credentialEncryptionService.Decrypt(credential.EncryptedAccessToken);

            await providerClient.DisconnectAsync(
                new ProviderDisconnectRequest(connection.Id, accessToken),
                cancellationToken);

            credential.Clear();
            connection.MarkDisconnected();

            await credentialRepository.UpdateAsync(credential, cancellationToken);
            await connectionRepository.UpdateAsync(connection, cancellationToken);

            logger.LogInformation("Disconnected integration connection {ConnectionId}", connection.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to disconnect integration connection {ConnectionId}", request.ConnectionId);
            connection.MarkFailed();
            await connectionRepository.UpdateAsync(connection, cancellationToken);

            return Result.Failure(new Error("INTEGRATION_DISCONNECT_FAILED", "Unable to disconnect integration."));
        }
    }
}
