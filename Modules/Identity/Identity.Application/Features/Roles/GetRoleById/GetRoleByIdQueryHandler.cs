using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Roles.GetRoleById;

public sealed class GetRoleByIdQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetByIdAsync(query.RoleId, cancellationToken);

        if (role is null)
        {
            return Result<RoleDto>.Failure(
                new Error("Role.NotFound", $"Role with id '{query.RoleId}' was not found."));
        }

        var dto = new RoleDto(
            role.Id,
            role.Code.Value,
            role.Name.Value);

        return Result<RoleDto>.Success(dto);
    }
}

