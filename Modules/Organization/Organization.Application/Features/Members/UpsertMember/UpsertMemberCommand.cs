using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.UpsertMember;

public sealed record UpsertMemberCommand(Guid OrganizationId, Guid UserId, Guid RoleId, Guid ActorUserId)
    : IAppRequest<Result<OrganizationMemberDto>>;
