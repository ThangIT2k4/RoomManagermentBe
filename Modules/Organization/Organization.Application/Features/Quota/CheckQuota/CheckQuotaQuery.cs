using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Quota.CheckQuota;

public sealed record CheckQuotaQuery(Guid OrganizationId, string FeatureKey, int CurrentUsage)
    : IAppRequest<Result<QuotaResultDto>>;
