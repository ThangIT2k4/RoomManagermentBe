using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Permissions.GetPermissionById;

public sealed class GetPermissionByIdQueryHandler(IPermissionRepository permissionRepository)
    : IRequestHandler<GetPermissionByIdQuery, Result<PermissionDto>>
{
    public async Task<Result<PermissionDto>> Handle(GetPermissionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var permission = await permissionRepository.GetByIdAsync(query.PermissionId, cancellationToken);

        if (permission is null)
        {
            return Result<PermissionDto>.Failure(
                new Error("Permission.NotFound", $"Permission with id '{query.PermissionId}' was not found."));
        }

        var dto = new PermissionDto(
            permission.Id,
            permission.Code.Value,
            permission.Name);

        return Result<PermissionDto>.Success(dto);
    }
}

