using CRM.Application.Features.Shared;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Leads.GetLeads;

public sealed record LeadListItemDto(Guid Id, Guid OrganizationId, string? FullName, string Status, DateTime CreatedAt);

public sealed record GetLeadsResult(PagedResult<LeadListItemDto> Data);

public sealed record GetLeadsQuery(
    Guid OrganizationId,
    string? Search = null,
    string? Status = null,
    PagingRequest? Paging = null)
    : IAppRequest<Result<GetLeadsResult>>;
