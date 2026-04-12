using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Leads.UpdateLeadStatus;

public sealed record UpdateLeadStatusRequest(Guid LeadId, string Status)
    : IAppRequest<Result<LeadDto>>;
