using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Settings.UpsertEmailSettings;

public sealed class UpsertEmailSettingsCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<UpsertEmailSettingsCommand, Result>
{
    public Task<Result> Handle(UpsertEmailSettingsCommand request, CancellationToken cancellationToken)
        => service.UpsertEmailSettingsAsync(
            new UpsertEmailSettingsRequest(request.OrganizationId, request.ActorUserId, request.FromName, request.FromEmail, request.Provider, request.SmtpHost, request.SmtpPort, request.SmtpEncryption, request.SmtpUsername, request.SmtpPassword),
            cancellationToken);
}
