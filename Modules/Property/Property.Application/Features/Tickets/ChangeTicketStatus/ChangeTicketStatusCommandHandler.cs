using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Tickets.ChangeTicketStatus;

public sealed class ChangeTicketStatusCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<ChangeTicketStatusCommand, TicketDto?>
{
    public Task<TicketDto?> Handle(ChangeTicketStatusCommand request, CancellationToken cancellationToken)
        => service.ChangeTicketStatusAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
