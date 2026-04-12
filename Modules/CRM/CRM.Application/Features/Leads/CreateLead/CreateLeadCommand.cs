using CRM.Application.Features.Leads;
using MediatR;

namespace CRM.Application.Features.Leads.CreateLead;

/// <summary>Command: tạo lead mới (ghi).</summary>
public sealed record CreateLeadCommand(Guid OrganizationId, string? FullName, string Status = "new")
    : IRequest<Result<LeadDto>>;
