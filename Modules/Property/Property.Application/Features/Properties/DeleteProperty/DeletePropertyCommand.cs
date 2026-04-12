using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Properties.DeleteProperty;

public sealed record DeletePropertyCommand(Guid OrganizationId, Guid UserId, Guid PropertyId)
    : IAppRequest<bool>;
