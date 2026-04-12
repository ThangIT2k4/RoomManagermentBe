using MediatR;

namespace CRM.Application.Features.Leads;

public sealed record LeadDto(
    Guid Id,
    Guid OrganizationId,
    string? FullName,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CreateLeadRequest(Guid OrganizationId, string? FullName, string Status = "new")
    : IRequest<Result<LeadDto>>;

public sealed record UpdateLeadStatusRequest(Guid LeadId, string Status)
    : IRequest<Result<LeadDto>>;
