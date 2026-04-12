using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.CancelViewing;

public sealed class CancelViewingCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<CancelViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(CancelViewingCommand request, CancellationToken cancellationToken)
        => crm.CancelViewingAsync(request, cancellationToken);
}
