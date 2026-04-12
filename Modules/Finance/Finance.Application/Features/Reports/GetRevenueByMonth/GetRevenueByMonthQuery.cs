using Finance.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Reports.GetRevenueByMonth;

public sealed record GetRevenueByMonthQuery(Guid OrganizationId, int Year)
    : IAppRequest<Result<IReadOnlyList<RevenueMonthDto>>>;
