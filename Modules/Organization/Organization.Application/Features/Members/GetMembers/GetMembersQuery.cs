using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.GetMembers;

public sealed record GetMembersQuery(Guid OrganizationId, int Page, int PageSize)
    : IAppRequest<Result<PagedResponse<OrganizationMemberDto>>>;
