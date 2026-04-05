namespace Finance.Domain.Repositories;

public sealed record PaymentSearchQuery(
    Guid OrganizationId,
    DateTime? FromPaidAtUtc,
    DateTime? ToPaidAtUtc,
    Guid? MethodId,
    string? Status,
    int Page,
    int PerPage);
