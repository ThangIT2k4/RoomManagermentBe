using Finance.Domain.Entities;

namespace Finance.Domain.Repositories;

public interface IInvoiceRepository
{
    Task<InvoiceEntity?> GetByIdAsync(Guid id, Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>Loads invoice if not soft-deleted (no organization filter — caller must authorize).</summary>
    Task<InvoiceEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(InvoiceEntity invoice, CancellationToken cancellationToken = default);

    Task UpdateAsync(InvoiceEntity invoice, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<InvoiceListRow> Rows, int TotalCount)> SearchAsync(
        InvoiceSearchQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Max numeric suffix for invoice numbers like INV2026-#### for this org/year.</summary>
    Task<int> GetMaxInvoiceSequenceForYearAsync(Guid organizationId, int year, CancellationToken cancellationToken = default);

    /// <summary>All orgs: <c>sent</c> invoices with due date strictly before <paramref name="asOfDate"/>.</summary>
    Task<IReadOnlyList<InvoiceEntity>> ListSentDueBeforeAsync(
        DateOnly asOfDate,
        CancellationToken cancellationToken = default);

    /// <summary>Tenant-visible invoices on the given leases (non-draft, not deleted).</summary>
    Task<IReadOnlyList<InvoiceEntity>> ListForLeaseIdsAsync(
        IReadOnlyList<Guid> leaseIds,
        CancellationToken cancellationToken = default);

    Task<DebtSummaryData> GetDebtSummaryAsync(Guid organizationId, DateOnly asOf, CancellationToken cancellationToken = default);
}
