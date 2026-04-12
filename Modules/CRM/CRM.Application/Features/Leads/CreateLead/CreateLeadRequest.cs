using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Leads.CreateLead;

public sealed record CreateLeadRequest(Guid OrganizationId, string? FullName, string Status = "new")
    : IAppRequest<Result<LeadDto>>;
