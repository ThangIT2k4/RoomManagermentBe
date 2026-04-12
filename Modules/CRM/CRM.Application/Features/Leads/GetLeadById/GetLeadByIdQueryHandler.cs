using CRM.Application.Features.Leads;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Features.Leads.GetLeadById;

public sealed class GetLeadByIdQueryHandler(ICrmApplicationService crm)
    : IRequestHandler<GetLeadByIdQuery, Result<LeadDto>>
{
    public Task<Result<LeadDto>> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
        => crm.GetLeadByIdAsync(request.LeadId, cancellationToken);
}
