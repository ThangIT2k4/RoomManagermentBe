using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Meters.AddMeterReading;

public sealed record AddMeterReadingCommand(Guid OrganizationId, Guid UserId, AddMeterReadingRequest Request)
    : IAppRequest<bool>;
