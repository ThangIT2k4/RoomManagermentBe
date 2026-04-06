using Auth.Application.Common;
using Auth.Application.Dtos;

namespace Auth.Application.Services;

/// <summary>
/// Cross-service organization membership (roles). Replace with a real gateway when the Organization service is wired.
/// </summary>
public interface IOrganizationMembershipGateway
{
    Task<Result> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);

    Task<Result> RemoveRoleAsync(RemoveRoleRequest request, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<RoleDto>>> GetUserRolesAsync(GetUserRolesRequest request, CancellationToken cancellationToken = default);
}
