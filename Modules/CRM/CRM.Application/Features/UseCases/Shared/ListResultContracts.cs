namespace CRM.Application.Features.UseCases;

public sealed record LeadListItemDto(Guid Id, Guid OrganizationId, string? FullName, string Status, DateTime CreatedAt);
public sealed record ViewingListItemDto(Guid Id, Guid LeadId, DateTime ScheduledAt, string? Location, string Status);
public sealed record BookingListItemDto(Guid Id, Guid LeadId, decimal DepositAmount, string Currency, string Status, DateTime CreatedAt);
public sealed record ReviewListItemDto(Guid Id, Guid ViewingId, short Rating, string Content, DateTime CreatedAt);
public sealed record CommissionPolicyListItemDto(Guid Id, Guid OrganizationId, string Name, string PolicyType, decimal Rate, DateTime EffectiveFrom, DateTime? EffectiveTo);
public sealed record CommissionEventListItemDto(Guid Id, Guid OrganizationId, Guid? AgentUserId, decimal Amount, string Status, DateTime CreatedAt);

public sealed record GetLeadsResult(PagedResult<LeadListItemDto> Data);
public sealed record GetViewingsResult(PagedResult<ViewingListItemDto> Data);
public sealed record GetBookingsResult(PagedResult<BookingListItemDto> Data);
public sealed record GetReviewsResult(PagedResult<ReviewListItemDto> Data);
public sealed record GetCommissionPoliciesResult(PagedResult<CommissionPolicyListItemDto> Data);
public sealed record GetCommissionEventsResult(PagedResult<CommissionEventListItemDto> Data);
