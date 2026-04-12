using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Tickets.ChangeTicketStatus;

public sealed record ChangeTicketStatusCommand(Guid OrganizationId, Guid UserId, TicketStatusRequest Request)
    : IAppRequest<TicketDto?>;
