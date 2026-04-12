using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Quota.CheckQuota;

public sealed class CheckQuotaQueryHandler(IOrganizationApplicationService service)
    : IAppRequestHandler<CheckQuotaQuery, Result<QuotaResultDto>>
{
    public Task<Result<QuotaResultDto>> Handle(CheckQuotaQuery request, CancellationToken cancellationToken)
        => service.CheckQuotaAsync(
            new CheckQuotaRequest(request.OrganizationId, request.FeatureKey, request.CurrentUsage),
            cancellationToken);
}
