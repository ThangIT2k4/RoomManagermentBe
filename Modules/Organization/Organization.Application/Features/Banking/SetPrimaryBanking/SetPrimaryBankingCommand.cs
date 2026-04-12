using Organization.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Banking.SetPrimaryBanking;

public sealed record SetPrimaryBankingCommand(Guid OrganizationId, Guid BankingId, Guid ActorUserId)
    : IAppRequest<Result>;
