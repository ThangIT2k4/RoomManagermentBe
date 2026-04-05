namespace Finance.Domain.Repositories;

public sealed record DebtSummaryData(
    int OverdueCount,
    decimal OverdueAmount,
    int DueSoonCount,
    decimal DueSoonAmount);
