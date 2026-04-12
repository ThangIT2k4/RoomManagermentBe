using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Common;

namespace Auth.Infrastructure.Services;

public sealed class NoOpOrganizationMembershipGateway : IOrganizationMembershipGateway
{
    private static readonly Error NotConfigured = Error.BadRequest(
        "Auth.Organization.GatewayNotConfigured",
        "Thành phần quản lý thành viên tổ chức chưa được cấu hình cho môi trường triển khai này.");

    public Task<Result> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(NotConfigured));

    public Task<Result> RemoveRoleAsync(RemoveRoleRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(NotConfigured));

    public Task<Result<IReadOnlyList<RoleDto>>> GetUserRolesAsync(
        GetUserRolesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Result<IReadOnlyList<RoleDto>>.Failure(NotConfigured));
}
