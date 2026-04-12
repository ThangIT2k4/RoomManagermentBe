using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Leads.GetLeadById;

public sealed record GetLeadByIdQuery(Guid LeadId) : IAppRequest<Result<LeadDto>>;
