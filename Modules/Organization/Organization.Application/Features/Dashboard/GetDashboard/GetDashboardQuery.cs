using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Dashboard.GetDashboard;

public sealed record GetDashboardQuery(Guid OrganizationId) : IAppRequest<Result<DashboardDto>>;
