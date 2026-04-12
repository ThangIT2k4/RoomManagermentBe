using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.ConfirmViewing;

public sealed class ConfirmViewingCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<ConfirmViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(ConfirmViewingCommand request, CancellationToken cancellationToken)
        => crm.ConfirmViewingAsync(request.ViewingId, cancellationToken);
}
