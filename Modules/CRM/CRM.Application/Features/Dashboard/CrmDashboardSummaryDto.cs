namespace CRM.Application.Features.Dashboard;

public sealed record CrmDashboardSummaryDto(
    int LeadsNew,
    int LeadsContacted,
    int LeadsQualified,
    int LeadsConverted,
    int LeadsLost,
    DateTime FetchedAt);
