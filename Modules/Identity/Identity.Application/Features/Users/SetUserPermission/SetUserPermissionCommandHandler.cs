using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Users.SetUserPermission;

public sealed class SetUserPermissionCommandHandler(
    IUserRepository userRepository,
    IPermissionRepository permissionRepository,
    IUserPermissionRepository userPermissionRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(SetUserPermissionCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                new Error("User.NotFound", $"User with id '{command.UserId}' was not found."));
        }

        var permission = await permissionRepository.GetByIdAsync(command.PermissionId, cancellationToken);
        if (permission is null)
        {
            return Result.Failure(
                new Error("Permission.NotFound", $"Permission with id '{command.PermissionId}' was not found."));
        }

        var existing = await userPermissionRepository.GetAsync(user.Id, permission.Id, cancellationToken);
        if (existing is null)
        {
            var userPermission = UserPermissionEntity.Create(user.Id, permission.Id, command.IsGranted);
            await userPermissionRepository.AddAsync(userPermission, cancellationToken);
        }
        else
        {
            existing.SetGranted(command.IsGranted);
            await userPermissionRepository.UpdateAsync(existing, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

