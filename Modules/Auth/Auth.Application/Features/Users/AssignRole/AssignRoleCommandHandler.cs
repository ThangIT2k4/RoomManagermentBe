using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Users.AssignRole;

public sealed class AssignRoleCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<AssignRoleCommand, Result>
{
    public Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        => authService.AssignRoleAsync(
            new AssignRoleRequest(request.OrganizationId, request.UserId, request.RoleId),
            cancellationToken);
}
