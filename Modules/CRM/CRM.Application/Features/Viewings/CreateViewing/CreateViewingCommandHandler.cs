using CRM.Application.Features.Viewings;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Features.Viewings.CreateViewing;

public sealed class CreateViewingCommandHandler(ICrmApplicationService crm)
    : IRequestHandler<CreateViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(CreateViewingCommand request, CancellationToken cancellationToken)
        => crm.CreateViewingAsync(request, cancellationToken);
}
