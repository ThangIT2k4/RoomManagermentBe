using CRM.Application.Features.Leads;
using MediatR;

namespace CRM.Application.Features.Leads.UpdateLeadStatus;

/// <summary>Command: cập nhật trạng thái lead (ghi).</summary>
public sealed record UpdateLeadStatusCommand(Guid LeadId, string Status)
    : IRequest<Result<LeadDto>>;
