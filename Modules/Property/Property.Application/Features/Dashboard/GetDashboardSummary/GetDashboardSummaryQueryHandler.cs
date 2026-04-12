using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Dashboard.GetDashboardSummary;

public sealed class GetDashboardSummaryQueryHandler(IPropertyApplicationService service)
    : IAppRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        => service.GetDashboardSummaryAsync(request.OrganizationId, cancellationToken);
}
