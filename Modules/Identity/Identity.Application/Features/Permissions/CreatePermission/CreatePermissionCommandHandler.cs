using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Features.Permissions.CreatePermission;

public sealed class CreatePermissionCommandHandler(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result<CreatePermissionResult>> HandleAsync(CreatePermissionCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await permissionRepository.GetByCodeAsync(command.Code, cancellationToken);
        if (existing is not null)
        {
            return Result<CreatePermissionResult>.Failure(
                new Error("Permission.DuplicateCode", "Permission code already exists."));
        }

        var code = PermissionCode.Create(command.Code);

        var permission = PermissionEntity.Create(Guid.NewGuid(), code, command.Name);

        permission = await permissionRepository.AddAsync(permission, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new CreatePermissionResult(
            permission.Id,
            permission.Code.Value,
            permission.Name);

        return Result<CreatePermissionResult>.Success(result);
    }
}

