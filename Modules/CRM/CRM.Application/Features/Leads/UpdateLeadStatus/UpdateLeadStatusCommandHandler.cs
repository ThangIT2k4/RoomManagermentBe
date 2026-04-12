using CRM.Application.Features.Leads;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Features.Leads.UpdateLeadStatus;

public sealed class UpdateLeadStatusCommandHandler(ICrmApplicationService crm)
    : IRequestHandler<UpdateLeadStatusCommand, Result<LeadDto>>
{
    public Task<Result<LeadDto>> Handle(UpdateLeadStatusCommand request, CancellationToken cancellationToken)
        => crm.UpdateLeadStatusAsync(request, cancellationToken);
}
