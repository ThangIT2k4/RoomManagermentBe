using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Leads.CreateLead;

public sealed class CreateLeadCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<CreateLeadRequest, Result<LeadDto>>
{
    public Task<Result<LeadDto>> Handle(CreateLeadRequest request, CancellationToken cancellationToken)
        => crm.CreateLeadAsync(request, cancellationToken);
}

