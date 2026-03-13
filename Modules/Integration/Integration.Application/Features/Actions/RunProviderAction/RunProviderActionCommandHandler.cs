using Integration.Application.Common;
using Integration.Application.Dtos;
using Integration.Application.Services;
using Integration.Domain.Entities;
using Integration.Domain.Providers;
using Integration.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Integration.Application.Features.Actions.RunProviderAction;

public sealed class RunProviderActionCommandHandler(
    IIntegrationConnectionRepository connectionRepository,
    IIntegrationCredentialRepository credentialRepository,
    IIntegrationJobRepository jobRepository,
    IIntegrationProviderClientFactory providerClientFactory,
    ICredentialEncryptionService credentialEncryptionService,
    ILogger<RunProviderActionCommandHandler> logger)
    : IRequestHandler<RunProviderActionCommand, Result<ProviderActionExecutionDto>>
{
    public async Task<Result<ProviderActionExecutionDto>> Handle(
        RunProviderActionCommand request,
        CancellationToken cancellationToken)
    {
        var connection = await connectionRepository.GetByIdAsync(request.ConnectionId, cancellationToken);
        if (connection is null)
        {
            return Result<ProviderActionExecutionDto>.Failure(
                new Error("INTEGRATION_NOT_FOUND", "Connection not found."));
        }

        var credential = await credentialRepository.GetByConnectionIdAsync(request.ConnectionId, cancellationToken);
        if (credential is null)
        {
            return Result<ProviderActionExecutionDto>.Failure(
                new Error("CREDENTIAL_NOT_FOUND", "Credential not found."));
        }

        var job = IntegrationJobEntity.Create(request.ConnectionId, request.ActionName, request.PayloadJson);
        await jobRepository.AddAsync(job, cancellationToken);

        try
        {
            var providerClient = providerClientFactory.GetClient(connection.Provider);
            var accessToken = credentialEncryptionService.Decrypt(credential.EncryptedAccessToken);

            var providerResult = await providerClient.ExecuteActionAsync(
                new ProviderActionRequest(
                    request.ConnectionId,
                    request.ActionName,
                    request.PayloadJson,
                    accessToken),
                cancellationToken);

            if (providerResult.IsSuccess)
            {
                job.MarkCompleted();
                connection.MarkSynced();
                await jobRepository.UpdateAsync(job, cancellationToken);
                await connectionRepository.UpdateAsync(connection, cancellationToken);

                return Result<ProviderActionExecutionDto>.Success(new ProviderActionExecutionDto
                {
                    JobId = job.Id,
                    IsSuccess = true,
                    Message = providerResult.Message,
                    ExternalJobId = providerResult.ExternalJobId
                });
            }

            job.MarkFailed(providerResult.Message);
            await jobRepository.UpdateAsync(job, cancellationToken);

            return Result<ProviderActionExecutionDto>.Failure(
                new Error("PROVIDER_ACTION_FAILED", providerResult.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to run action {ActionName} on connection {ConnectionId}", request.ActionName, request.ConnectionId);

            job.MarkFailed(ex.Message);
            await jobRepository.UpdateAsync(job, cancellationToken);

            return Result<ProviderActionExecutionDto>.Failure(
                new Error("PROVIDER_ACTION_EXCEPTION", "Unexpected error while running provider action."));
        }
    }
}
