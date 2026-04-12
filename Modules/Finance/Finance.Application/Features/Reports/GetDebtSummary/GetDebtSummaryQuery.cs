using Finance.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Reports.GetDebtSummary;

public sealed record GetDebtSummaryQuery(Guid OrganizationId)
    : IAppRequest<Result<DebtSummaryDto>>;
