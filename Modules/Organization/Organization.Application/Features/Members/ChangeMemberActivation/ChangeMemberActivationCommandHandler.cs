using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.ChangeMemberActivation;

public sealed class ChangeMemberActivationCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<ChangeMemberActivationCommand, Result>
{
    public Task<Result> Handle(ChangeMemberActivationCommand request, CancellationToken cancellationToken)
        => service.ChangeMemberActivationAsync(
            new ChangeMemberActivationRequest(request.OrganizationId, request.UserId, request.IsActive, request.ActorUserId, request.Reason),
            cancellationToken);
}
