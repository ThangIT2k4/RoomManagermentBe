using Finance.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.DepositRefunds.ConfirmDepositRefund;

public sealed record ConfirmDepositRefundCommand(
    Guid OrganizationId,
    Guid RefundId,
    Guid UserId,
    DateTime PaidAtUtc,
    string? ReferenceCode)
    : IAppRequest<Result<DepositRefundDto>>;
