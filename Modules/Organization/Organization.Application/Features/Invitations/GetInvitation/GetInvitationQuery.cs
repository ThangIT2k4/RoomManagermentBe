using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.GetInvitation;

public sealed record GetInvitationQuery(string Token) : IAppRequest<Result<InvitationPreviewDto>>;
