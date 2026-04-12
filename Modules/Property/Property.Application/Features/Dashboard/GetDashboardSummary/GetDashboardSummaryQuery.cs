using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Dashboard.GetDashboardSummary;

public sealed record GetDashboardSummaryQuery(Guid OrganizationId) : IAppRequest<DashboardSummaryDto>;
