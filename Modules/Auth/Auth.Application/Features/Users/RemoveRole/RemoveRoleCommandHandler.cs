using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Users.RemoveRole;

public sealed class RemoveRoleCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<RemoveRoleCommand, Result>
{
    public Task<Result> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
        => authService.RemoveRoleAsync(
            new RemoveRoleRequest(request.OrganizationId, request.UserId),
            cancellationToken);
}
