using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.Roles.GetUserRoles;

public sealed class GetUserRolesQueryHandler(IAuthApplicationService authService)
    : IRequestHandler<GetUserRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    public Task<Result<IReadOnlyList<RoleDto>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        => authService.GetUserRolesAsync(new GetUserRolesRequest(request.UserId, request.OrganizationId), cancellationToken);
}
