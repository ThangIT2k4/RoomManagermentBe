using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Tickets.CreateTicket;

public sealed class CreateTicketCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<CreateTicketCommand, TicketDto>
{
    public Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        => service.CreateTicketAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
