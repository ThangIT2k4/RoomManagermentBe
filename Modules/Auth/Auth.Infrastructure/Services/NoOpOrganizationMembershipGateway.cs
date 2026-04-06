using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;

namespace Auth.Infrastructure.Services;

public sealed class NoOpOrganizationMembershipGateway : IOrganizationMembershipGateway
{
    private static readonly Error NotConfigured = new(
        "Auth.Organization.GatewayNotConfigured",
        "Organization membership is not configured for this deployment.");

    public Task<Result> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(NotConfigured));

    public Task<Result> RemoveRoleAsync(RemoveRoleRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(NotConfigured));

    public Task<Result<IReadOnlyList<RoleDto>>> GetUserRolesAsync(
        GetUserRolesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Result<IReadOnlyList<RoleDto>>.Failure(NotConfigured));
}
