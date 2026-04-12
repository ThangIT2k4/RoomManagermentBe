using Finance.Application.Dtos;
using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Reports.GetRevenueByMonth;

public sealed class GetRevenueByMonthQueryHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<GetRevenueByMonthQuery, Result<IReadOnlyList<RevenueMonthDto>>>
{
    public Task<Result<IReadOnlyList<RevenueMonthDto>>> Handle(GetRevenueByMonthQuery request, CancellationToken cancellationToken)
        => finance.GetRevenueByMonthAsync(request.OrganizationId, request.Year, cancellationToken);
}
