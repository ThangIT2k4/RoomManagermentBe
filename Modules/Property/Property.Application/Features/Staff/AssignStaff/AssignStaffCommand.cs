using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Staff.AssignStaff;

public sealed record AssignStaffCommand(Guid OrganizationId, Guid UserId, AssignPropertyStaffRequest Request)
    : IAppRequest<PropertyStaffDto>;
