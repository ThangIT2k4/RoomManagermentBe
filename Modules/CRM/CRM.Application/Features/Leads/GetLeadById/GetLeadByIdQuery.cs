using CRM.Application.Features.Leads;
using MediatR;

namespace CRM.Application.Features.Leads.GetLeadById;

/// <summary>Query: đọc một lead theo id.</summary>
public sealed record GetLeadByIdQuery(Guid LeadId) : IRequest<Result<LeadDto>>;
