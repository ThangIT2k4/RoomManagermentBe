using CRM.Application.Features.Leads.GetLeads;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Features.Leads.GetLeads;

public sealed class GetLeadsQueryHandler(ICrmApplicationService crm)
    : IRequestHandler<GetLeadsQuery, Result<GetLeadsResult>>
{
    public Task<Result<GetLeadsResult>> Handle(GetLeadsQuery request, CancellationToken cancellationToken)
        => crm.GetLeadsAsync(request, cancellationToken);
}
