using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.ChangeMemberRole;

public sealed class ChangeMemberRoleCommandHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<ChangeMemberRoleCommand, Result>
{
    public Task<Result> Handle(ChangeMemberRoleCommand request, CancellationToken cancellationToken)
        => service.ChangeMemberRoleAsync(
            new ChangeMemberRoleRequest(request.OrganizationId, request.UserId, request.RoleId, request.ActorUserId),
            cancellationToken);
}
