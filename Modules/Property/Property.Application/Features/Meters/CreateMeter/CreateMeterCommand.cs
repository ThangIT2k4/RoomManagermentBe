using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Meters.CreateMeter;

public sealed record CreateMeterCommand(Guid OrganizationId, Guid UserId, CreateMeterRequest Request)
    : IAppRequest<MeterDto>;
