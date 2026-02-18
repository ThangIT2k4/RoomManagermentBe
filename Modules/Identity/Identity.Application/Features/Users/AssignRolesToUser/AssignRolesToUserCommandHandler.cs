using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Users.AssignRolesToUser;

public sealed class AssignRolesToUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AssignRolesToUserCommand, Result>
{
    public async Task<Result> Handle(AssignRolesToUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                new Error("User.NotFound", $"User with id '{command.UserId}' was not found."));
        }

        foreach (var roleId in command.RoleIds.Distinct())
        {
            var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role is null)
            {
                return Result.Failure(
                    new Error("Role.NotFound", $"Role with id '{roleId}' was not found."));
            }

            var existing = await userRoleRepository.GetAsync(user.Id, role.Id, cancellationToken);
            if (existing is not null)
            {
                continue;
            }

            var userRole = UserRoleEntity.Create(user.Id, role.Id);
            await userRoleRepository.AddAsync(userRole, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

