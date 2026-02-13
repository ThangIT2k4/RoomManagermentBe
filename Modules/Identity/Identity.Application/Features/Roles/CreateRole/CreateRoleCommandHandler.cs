using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Features.Roles.CreateRole;

public sealed class CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result<CreateRoleResult>> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await roleRepository.GetByCodeAsync(command.Code, cancellationToken);
        if (existing is not null)
        {
            return Result<CreateRoleResult>.Failure(
                new Error("Role.DuplicateCode", "Role code already exists."));
        }

        var code = RoleCode.Create(command.Code);
        var name = RoleName.Create(command.Name);

        var role = RoleEntity.Create(Guid.NewGuid(), code, name);

        role = await roleRepository.AddAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new CreateRoleResult(
            role.Id,
            role.Code.Value,
            role.Name.Value);

        return Result<CreateRoleResult>.Success(result);
    }
}

