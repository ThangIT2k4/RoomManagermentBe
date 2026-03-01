using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Permissions.GetPermissionById;

public sealed record GetPermissionByIdQuery(Guid PermissionId) : IRequest<Result<PermissionDto>>;

