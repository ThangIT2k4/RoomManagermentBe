using Finance.Application.Dtos;
using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.DepositRefunds.ForfeitDepositRefund;

public sealed class ForfeitDepositRefundCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<ForfeitDepositRefundCommand, Result<DepositRefundDto>>
{
    public Task<Result<DepositRefundDto>> Handle(ForfeitDepositRefundCommand request, CancellationToken cancellationToken)
        => finance.ForfeitDepositRefundAsync(
            request.OrganizationId,
            request.RefundId,
            new ForfeitDepositRefundRequest(request.OrganizationId, request.UserId, request.Reason),
            cancellationToken);
}
