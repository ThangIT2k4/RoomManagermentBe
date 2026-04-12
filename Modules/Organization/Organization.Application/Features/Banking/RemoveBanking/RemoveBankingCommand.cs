using Organization.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Banking.RemoveBanking;

public sealed record RemoveBankingCommand(Guid OrganizationId, Guid BankingId, Guid ActorUserId)
    : IAppRequest<Result>;
