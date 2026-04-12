using CRM.Application.Features.Viewings;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Features.Viewings.CancelViewing;

public sealed class CancelViewingCommandHandler(ICrmApplicationService crm)
    : IRequestHandler<CancelViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(CancelViewingCommand request, CancellationToken cancellationToken)
        => crm.CancelViewingAsync(request, cancellationToken);
}
