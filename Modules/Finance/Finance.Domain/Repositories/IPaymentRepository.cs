using Finance.Domain.Entities;

namespace Finance.Domain.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(PaymentEntity payment, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentEntity>> ListByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByReferenceCodeAsync(string referenceCode, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<PaymentListRow> Rows, int TotalCount)> SearchAsync(
        PaymentSearchQuery query,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentEntity>> ListSuccessByLeaseIdsAsync(
        IReadOnlyList<Guid> leaseIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MonthlyRevenueAggregate>> GetMonthlyRevenueForYearAsync(
        Guid organizationId,
        int year,
        CancellationToken cancellationToken = default);
}
