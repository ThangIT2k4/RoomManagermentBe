using Finance.Domain.Entities;

namespace Finance.Domain.Repositories;

public interface IInvoiceItemRepository
{
    Task<IReadOnlyList<InvoiceItemEntity>> ListActiveByInvoiceIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<InvoiceItemEntity> items, CancellationToken cancellationToken = default);

    Task SoftDeleteByInvoiceIdAsync(Guid invoiceId, Guid deletedBy, CancellationToken cancellationToken = default);
}
