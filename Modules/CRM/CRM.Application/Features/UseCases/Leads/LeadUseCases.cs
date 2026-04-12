using CRM.Application.Features.Leads;
using MediatR;

namespace CRM.Application.Features.UseCases;

public sealed record CreateLeadCommand(Guid OrganizationId, string? FullName, string Status = "new", Guid? AssignedUserId = null);
public sealed record UpdateLeadCommand(Guid LeadId, string? FullName, Guid? AssignedUserId = null);
public sealed record DeleteLeadCommand(Guid LeadId, Guid RequestedBy);
public sealed record GetLeadsQuery(Guid OrganizationId, string? Search = null, string? Status = null, PagingRequest? Paging = null)
    : IRequest<Result<GetLeadsResult>>;
public sealed record GetLeadByIdQuery(Guid LeadId) : IRequest<Result<LeadDto>>;
public sealed record AssignLeadCommand(Guid LeadId, Guid AssigneeUserId, Guid RequestedBy);
public sealed record UpdateLeadStatusCommand(Guid LeadId, string Status, Guid RequestedBy);
