using CRM.Application.Features.Viewings;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Features.Viewings.CompleteViewing;

public sealed class CompleteViewingCommandHandler(ICrmApplicationService crm)
    : IRequestHandler<CompleteViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(CompleteViewingCommand request, CancellationToken cancellationToken)
        => crm.CompleteViewingAsync(request, cancellationToken);
}
