namespace CRM.Application.Features.UseCases;

public sealed record CreateCommissionPolicyCommand(Guid OrganizationId, string Name, string PolicyType, decimal Rate, DateTime EffectiveFrom, DateTime? EffectiveTo = null, Guid? CreatedBy = null);
public sealed record UpdateCommissionPolicyCommand(Guid PolicyId, string? Name = null, decimal? Rate = null, DateTime? EffectiveFrom = null, DateTime? EffectiveTo = null, Guid? UpdatedBy = null);
public sealed record DeleteCommissionPolicyCommand(Guid PolicyId, Guid RequestedBy);
public sealed record GetCommissionPoliciesQuery(Guid OrganizationId, string? Search = null, PagingRequest? Paging = null);
public sealed record GetCommissionPolicyByIdQuery(Guid PolicyId);
public sealed record GenerateCommissionEventCommand(Guid BookingId, Guid PolicyId, decimal BaseAmount, Guid RequestedBy);
public sealed record UpdateCommissionEventCommand(Guid CommissionEventId, decimal? Amount = null, string? Note = null, Guid? UpdatedBy = null);
public sealed record GetCommissionEventsQuery(Guid OrganizationId, Guid? AgentUserId = null, string? Status = null, PagingRequest? Paging = null);
public sealed record GetCommissionEventByIdQuery(Guid CommissionEventId);
public sealed record ApproveCommissionCommand(Guid CommissionEventId, Guid ApprovedBy, DateTime? ApprovedAt = null);
public sealed record PayCommissionCommand(Guid CommissionEventId, DateTime PaidAt, string? PaymentReference, Guid PaidBy);
