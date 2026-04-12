using CRM.Application.Features.Viewings;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Features.Viewings.ConfirmViewing;

public sealed class ConfirmViewingCommandHandler(ICrmApplicationService crm)
    : IRequestHandler<ConfirmViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(ConfirmViewingCommand request, CancellationToken cancellationToken)
        => crm.ConfirmViewingAsync(request.ViewingId, cancellationToken);
}
