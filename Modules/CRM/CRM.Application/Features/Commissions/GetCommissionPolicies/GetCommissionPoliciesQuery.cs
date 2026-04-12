using CRM.Application.Features.Shared;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Commissions.GetCommissionPolicies;

public sealed record CommissionPolicyListItemDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string PolicyType,
    decimal Rate,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo);

public sealed record GetCommissionPoliciesResult(PagedResult<CommissionPolicyListItemDto> Data);

public sealed record GetCommissionPoliciesQuery(
    Guid OrganizationId,
    string? Search = null,
    PagingRequest? Paging = null)
    : IAppRequest<Result<GetCommissionPoliciesResult>>;
