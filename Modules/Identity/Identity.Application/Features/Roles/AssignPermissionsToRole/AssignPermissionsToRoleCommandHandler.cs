using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Roles.AssignPermissionsToRole;

public sealed class AssignPermissionsToRoleCommandHandler(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IRolePermissionRepository rolePermissionRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(AssignPermissionsToRoleCommand command, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetByIdAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            return Result.Failure(
                new Error("Role.NotFound", $"Role with id '{command.RoleId}' was not found."));
        }

        foreach (var permissionId in command.PermissionIds.Distinct())
        {
            var permission = await permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (permission is null)
            {
                return Result.Failure(
                    new Error("Permission.NotFound", $"Permission with id '{permissionId}' was not found."));
            }

            var existing = await rolePermissionRepository.GetAsync(role.Id, permission.Id, cancellationToken);
            if (existing is not null)
            {
                continue;
            }

            var rolePermission = RolePermissionEntity.Create(role.Id, permission.Id);
            await rolePermissionRepository.AddAsync(rolePermission, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

