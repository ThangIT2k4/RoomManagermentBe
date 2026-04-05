namespace Finance.Domain.Repositories;

public sealed record MonthlyRevenueAggregate(int Month, decimal TotalRevenue, int InvoiceCount, int LeaseCount);
