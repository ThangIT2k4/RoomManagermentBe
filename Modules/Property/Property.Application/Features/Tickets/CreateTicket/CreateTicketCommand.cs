using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Tickets.CreateTicket;

public sealed record CreateTicketCommand(Guid OrganizationId, Guid UserId, CreateTicketRequest Request)
    : IAppRequest<TicketDto>;
