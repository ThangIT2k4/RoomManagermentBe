// using Auth.Application.Common;
// using Auth.Application.Dtos;
// using Auth.Application.Services;
// using RoomManagerment.Organizations.DatabaseSpecific;
// using RoomManagerment.Organizations.Linq;
// using SD.LLBLGen.Pro.LinqSupportClasses;
// using OrgDataAccessAdapter = RoomManagerment.Organizations.DatabaseSpecific.DataAccessAdapter;
// using OrgLinqMetaData = RoomManagerment.Organizations.Linq.LinqMetaData;
// using OrgUserEntity = RoomManagerment.Organizations.EntityClasses.OrganizationUserEntity;
//
// namespace Auth.Infrastructure.Services;
//
// public sealed class OrganizationMembershipGateway : IOrganizationMembershipGateway
// {
//     public async Task<Result> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default)
//     {
//         using var orgAdapter = new OrgDataAccessAdapter();
//         var orgLinq = new OrgLinqMetaData(orgAdapter);
//
//         var orgUser = await orgLinq.OrganizationUser
//             .Where(x => x.OrganizationId == request.OrganizationId && x.UserId == request.UserId)
//             .FirstOrDefaultAsync(cancellationToken);
//
//         if (orgUser is null)
//         {
//             orgUser = new OrgUserEntity
//             {
//                 Id = Guid.NewGuid(),
//                 OrganizationId = request.OrganizationId,
//                 UserId = request.UserId,
//                 RoleId = request.RoleId,
//                 IsActive = true,
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow
//             };
//         }
//         else
//         {
//             orgUser.RoleId = request.RoleId;
//             orgUser.UpdatedAt = DateTime.UtcNow;
//         }
//
//         await orgAdapter.SaveEntityAsync(orgUser, true, false, cancellationToken);
//         return Result.Success();
//     }
//
//     public async Task<Result> RemoveRoleAsync(RemoveRoleRequest request, CancellationToken cancellationToken = default)
//     {
//         using var orgAdapter = new OrgDataAccessAdapter();
//         var orgLinq = new OrgLinqMetaData(orgAdapter);
//
//         var orgUser = await orgLinq.OrganizationUser
//             .Where(x => x.OrganizationId == request.OrganizationId && x.UserId == request.UserId)
//             .FirstOrDefaultAsync(cancellationToken);
//
//         if (orgUser is null)
//         {
//             return Result.Success();
//         }
//
//         orgUser.RoleId = null;
//         orgUser.UpdatedAt = DateTime.UtcNow;
//         await orgAdapter.SaveEntityAsync(orgUser, true, false, cancellationToken);
//         return Result.Success();
//     }
//
//     public async Task<Result<IReadOnlyList<RoleDto>>> GetUserRolesAsync(
//         GetUserRolesRequest request,
//         CancellationToken cancellationToken = default)
//     {
//         using var orgAdapter = new OrgDataAccessAdapter();
//         var orgLinq = new OrgLinqMetaData(orgAdapter);
//
//         var query = orgLinq.OrganizationUser.Where(x => x.UserId == request.UserId && x.RoleId != null);
//         if (request.OrganizationId.HasValue)
//         {
//             query = query.Where(x => x.OrganizationId == request.OrganizationId.Value);
//         }
//
//         var roleIds = await query.Select(x => x.RoleId!.Value).Distinct().ToListAsync(cancellationToken);
//         if (roleIds.Count == 0)
//         {
//             return Result<IReadOnlyList<RoleDto>>.Success(Array.Empty<RoleDto>());
//         }
//
//         using var authAdapter = new RoomManagerment.Auth.DatabaseSpecific.DataAccessAdapter();
//         var authLinq = new RoomManagerment.Auth.Linq.LinqMetaData(authAdapter);
//         var roles = await authLinq.Role.Where(x => roleIds.Contains(x.Id)).ToListAsync(cancellationToken);
//         var result = roles.Select(x => new RoleDto(x.Id, x.KeyCode, x.Name, x.Description)).ToList();
//         return Result<IReadOnlyList<RoleDto>>.Success(result);
//     }
// }
