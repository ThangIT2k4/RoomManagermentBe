using CRM.Application.Features.Shared;
using CRM.Application.Features.Leads;
using MediatR;

namespace CRM.Application.Features.Leads.GetLeads;

public sealed record LeadListItemDto(Guid Id, Guid OrganizationId, string? FullName, string Status, DateTime CreatedAt);

public sealed record GetLeadsResult(PagedResult<LeadListItemDto> Data);

/// <summary>Query: danh sách lead (đọc).</summary>
public sealed record GetLeadsQuery(Guid OrganizationId, string? Search = null, string? Status = null, PagingRequest? Paging = null)
    : IRequest<Result<GetLeadsResult>>;
