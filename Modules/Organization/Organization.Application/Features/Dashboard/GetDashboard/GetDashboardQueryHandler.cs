using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Dashboard.GetDashboard;

public sealed class GetDashboardQueryHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<GetDashboardQuery, Result<DashboardDto>>
{
    public Task<Result<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        => service.GetDashboardAsync(request.OrganizationId, cancellationToken);
}
