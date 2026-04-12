using Finance.Application.Dtos;
using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Reports.GetDebtSummary;

public sealed class GetDebtSummaryQueryHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<GetDebtSummaryQuery, Result<DebtSummaryDto>>
{
    public Task<Result<DebtSummaryDto>> Handle(GetDebtSummaryQuery request, CancellationToken cancellationToken)
        => finance.GetDebtSummaryAsync(request.OrganizationId, cancellationToken);
}
