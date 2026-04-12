using CRM.Application.Features.Leads;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Features.Leads.CreateLead;

public sealed class CreateLeadCommandHandler(ICrmApplicationService crm)
    : IRequestHandler<CreateLeadCommand, Result<LeadDto>>
{
    public Task<Result<LeadDto>> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
        => crm.CreateLeadAsync(request, cancellationToken);
}
