using Finance.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.DepositRefunds.CreateDepositRefund;

public sealed record CreateDepositRefundCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid LeaseId,
    decimal Amount,
    string? Notes)
    : IAppRequest<Result<DepositRefundDto>>;
