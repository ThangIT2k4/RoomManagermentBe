using Finance.Application.Dtos;
using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.DepositRefunds.ConfirmDepositRefund;

public sealed class ConfirmDepositRefundCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<ConfirmDepositRefundCommand, Result<DepositRefundDto>>
{
    public Task<Result<DepositRefundDto>> Handle(ConfirmDepositRefundCommand request, CancellationToken cancellationToken)
        => finance.ConfirmDepositRefundPaidAsync(
            request.OrganizationId,
            request.RefundId,
            new ConfirmDepositRefundRequest(request.OrganizationId, request.UserId, request.PaidAtUtc, request.ReferenceCode),
            cancellationToken);
}
