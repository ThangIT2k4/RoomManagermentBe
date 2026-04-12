using Finance.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.DepositRefunds.ForfeitDepositRefund;

public sealed record ForfeitDepositRefundCommand(
    Guid OrganizationId,
    Guid RefundId,
    Guid UserId,
    string Reason)
    : IAppRequest<Result<DepositRefundDto>>;
