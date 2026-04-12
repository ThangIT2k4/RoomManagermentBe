using CRM.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.CreateViewing;

public sealed class CreateViewingCommandHandler(ICrmApplicationService crm)
    : IAppRequestHandler<CreateViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(CreateViewingCommand request, CancellationToken cancellationToken)
        => crm.CreateViewingAsync(request, cancellationToken);
}
