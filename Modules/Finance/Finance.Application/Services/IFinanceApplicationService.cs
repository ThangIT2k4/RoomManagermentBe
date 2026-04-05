using Finance.Application.Common;
using Finance.Application.Dtos;

namespace Finance.Application.Services;

public interface IFinanceApplicationService
{
    Task<Result<InvoiceDto>> CreateManualInvoiceAsync(
        Guid organizationId,
        Guid userId,
        Guid leaseId,
        DateOnly invoiceDate,
        DateOnly dueDate,
        string? notes,
        IReadOnlyList<InvoiceItemLineDto> items,
        CancellationToken cancellationToken = default);

    Task<Result<InvoiceDto>> UpdateDraftInvoiceAsync(
        Guid organizationId,
        Guid userId,
        Guid invoiceId,
        DateOnly dueDate,
        string? notes,
        IReadOnlyList<InvoiceItemLineDto> items,
        CancellationToken cancellationToken = default);

    Task<Result<InvoiceDto>> PublishInvoiceAsync(
        Guid organizationId,
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    Task<Result<InvoiceDto>> CancelInvoiceAsync(
        Guid organizationId,
        Guid userId,
        Guid invoiceId,
        string? reason,
        CancellationToken cancellationToken = default);

    Task<Result<InvoiceDto>> GetInvoiceAsync(
        Guid organizationId,
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    Task<Result<InvoiceDto>> GetInvoiceForTenantAsync(
        Guid tenantUserId,
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    Task<Result<PagedInvoicesDto>> SearchInvoicesAsync(
        Guid organizationId,
        IReadOnlyList<string>? statuses,
        Guid? leaseId,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? search,
        int page,
        int perPage,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<InvoiceDto>>> ListTenantInvoicesAsync(
        Guid tenantUserId,
        CancellationToken cancellationToken = default);

    Task<Result<InvoiceDto>> RecordManualPaymentAsync(
        RecordManualPaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<PagedPaymentsDto>> SearchPaymentsAsync(
        Guid organizationId,
        DateTime? fromPaidAtUtc,
        DateTime? toPaidAtUtc,
        Guid? methodId,
        string? status,
        int page,
        int perPage,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<PaymentDto>>> ListTenantPaymentsAsync(
        Guid tenantUserId,
        CancellationToken cancellationToken = default);

    Task<Result<OnlinePaymentInitiationResult>> InitiateOnlinePaymentAsync(
        Guid tenantUserId,
        Guid invoiceId,
        string methodKey,
        CancellationToken cancellationToken = default);

    Task<Result> HandlePaymentWebhookAsync(
        string rawBody,
        IReadOnlyDictionary<string, string> headers,
        CancellationToken cancellationToken = default);

    Task<Result<DepositRefundDto>> CreateDepositRefundAsync(
        CreateDepositRefundRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<DepositRefundDto>> ConfirmDepositRefundPaidAsync(
        Guid organizationId,
        Guid refundId,
        ConfirmDepositRefundRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<DepositRefundDto>> ForfeitDepositRefundAsync(
        Guid organizationId,
        Guid refundId,
        ForfeitDepositRefundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>UC-F02 skeleton: logs and returns count (lease integration later).</summary>
    Task<Result<int>> RunAutoInvoiceGenerationAsync(DateOnly runDate, CancellationToken cancellationToken = default);

    /// <summary>UC-F09: mark overdue + optional notifications.</summary>
    Task<Result<int>> RunOverdueSweepAsync(DateOnly asOfDate, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<RevenueMonthDto>>> GetRevenueByMonthAsync(
        Guid organizationId,
        int year,
        CancellationToken cancellationToken = default);

    Task<Result<DebtSummaryDto>> GetDebtSummaryAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
