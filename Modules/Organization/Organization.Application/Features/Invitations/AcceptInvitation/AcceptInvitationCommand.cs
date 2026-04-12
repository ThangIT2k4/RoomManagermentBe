using Organization.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Invitations.AcceptInvitation;

public sealed record AcceptInvitationCommand(string Token, Guid UserId) : IAppRequest<Result>;
