namespace CRM.Application.Features.UseCases;

public sealed record CreateLeadCommand(Guid OrganizationId, string? FullName, string Status = "new", Guid? AssignedUserId = null);
public sealed record UpdateLeadCommand(Guid LeadId, string? FullName, Guid? AssignedUserId = null);
public sealed record DeleteLeadCommand(Guid LeadId, Guid RequestedBy);
public sealed record GetLeadsQuery(Guid OrganizationId, string? Search = null, string? Status = null, PagingRequest? Paging = null);
public sealed record GetLeadByIdQuery(Guid LeadId);
public sealed record AssignLeadCommand(Guid LeadId, Guid AssigneeUserId, Guid RequestedBy);
public sealed record UpdateLeadStatusCommand(Guid LeadId, string Status, Guid RequestedBy);
