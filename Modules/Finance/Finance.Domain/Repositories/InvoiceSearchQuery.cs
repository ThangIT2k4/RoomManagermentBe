namespace Finance.Domain.Repositories;

public sealed record InvoiceSearchQuery(
    Guid OrganizationId,
    IReadOnlyList<string>? Statuses,
    Guid? LeaseId,
    DateOnly? FromDate,
    DateOnly? ToDate,
    string? Search,
    int Page,
    int PerPage);
