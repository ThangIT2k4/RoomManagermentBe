using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Properties.CreateProperty;

public sealed record CreatePropertyCommand(Guid OrganizationId, Guid UserId, CreatePropertyRequest Request)
    : IAppRequest<PropertyDto>;
