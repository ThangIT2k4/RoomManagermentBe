using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Banking.AddBanking;

public sealed record AddOrganizationBankingCommand(
    Guid OrganizationId,
    Guid ActorUserId,
    Guid? SepayBankId,
    string AccountNumber,
    string AccountHolderName,
    string? BranchName,
    string? BranchCode,
    string? SwiftCode,
    bool IsPrimary) : IAppRequest<Result<OrganizationBankingDto>>;
