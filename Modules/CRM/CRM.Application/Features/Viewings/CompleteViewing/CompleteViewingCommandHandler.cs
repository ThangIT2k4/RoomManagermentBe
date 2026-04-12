using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.CompleteViewing;

public sealed class CompleteViewingCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<CompleteViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(CompleteViewingCommand request, CancellationToken cancellationToken)
        => crm.CompleteViewingAsync(request, cancellationToken);
}
