using Finance.Application.Dtos;
using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.DepositRefunds.CreateDepositRefund;

public sealed class CreateDepositRefundCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<CreateDepositRefundCommand, Result<DepositRefundDto>>
{
    public Task<Result<DepositRefundDto>> Handle(CreateDepositRefundCommand request, CancellationToken cancellationToken)
        => finance.CreateDepositRefundAsync(
            new CreateDepositRefundRequest(
                request.OrganizationId,
                request.UserId,
                request.LeaseId,
                request.Amount,
                request.Notes),
            cancellationToken);
}
