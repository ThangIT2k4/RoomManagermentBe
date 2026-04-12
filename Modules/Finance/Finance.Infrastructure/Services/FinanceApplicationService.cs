using System.Text.Json;
using Finance.Application.Dtos;
using Finance.Application.Services;
using Finance.Domain;
using Finance.Domain.Entities;
using Finance.Domain.Events;
using Finance.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Services;

public sealed class FinanceApplicationService(
    IInvoiceRepository invoiceRepository,
    IInvoiceItemRepository invoiceItemRepository,
    IPaymentRepository paymentRepository,
    IDepositRefundRepository depositRefundRepository,
    ILeaseReadGateway leaseReadGateway,
    IInvoiceNumberGenerator invoiceNumberGenerator,
    IFinanceIntegrationEventPublisher integrationEventPublisher,
    IOnlinePaymentInitiator onlinePaymentInitiator,
    IPaymentWebhookHandler paymentWebhookHandler,
    ILogger<FinanceApplicationService> logger) : IFinanceApplicationService
{
    public async Task<Result<InvoiceDto>> CreateManualInvoiceAsync(
        Guid organizationId,
        Guid userId,
        Guid leaseId,
        DateOnly invoiceDate,
        DateOnly dueDate,
        string? notes,
        IReadOnlyList<InvoiceItemLineDto> items,
        CancellationToken cancellationToken = default)
    {
        if (items.Count == 0)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Items", "At least one line item is required."));
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (invoiceDate > today.AddDays(30))
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Date", "Invoice date cannot be more than 30 days in the future."));
        }

        var lease = await leaseReadGateway.GetLeaseAsync(leaseId, organizationId, cancellationToken);
        if (lease is null)
        {
            return Result<InvoiceDto>.Failure(Error.NotFound("Finance.Lease.NotFound", "Lease was not found for this organization."));
        }

        if (!lease.Status.Equals("active", StringComparison.OrdinalIgnoreCase))
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Lease.Inactive", "Lease must be active to create an invoice."));
        }

        if (items.Any(i => i.Quantity < 0 || i.UnitPrice < 0))
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Items", "Line quantities and unit prices must be non-negative."));
        }

        var total = items.Sum(i => Math.Round(i.Quantity * i.UnitPrice, 2, MidpointRounding.AwayFromZero));

        if (total <= 0)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Total", "Total amount must be greater than zero."));
        }

        List<InvoiceItemEntity> lineEntities;
        try
        {
            var invoice = InvoiceEntity.CreateDraft(
                organizationId,
                leaseId,
                invoiceDate,
                dueDate,
                total,
                notes,
                userId,
                isAutoCreated: false);

            var invoiceNo = await invoiceNumberGenerator.NextAsync(organizationId, invoiceDate, cancellationToken);
            invoice.SetInvoiceNo(invoiceNo);

            await invoiceRepository.AddAsync(invoice, cancellationToken);

            lineEntities = new List<InvoiceItemEntity>();
            foreach (var line in items)
            {
                lineEntities.Add(
                    InvoiceItemEntity.Create(
                        invoice.Id,
                        line.ItemType,
                        line.Description,
                        line.Quantity,
                        line.UnitPrice,
                        line.ServiceId,
                        line.MeterReadingId,
                        line.TicketLogId));
            }

            await invoiceItemRepository.AddRangeAsync(lineEntities, cancellationToken);
            return Result<InvoiceDto>.Success(await MapInvoiceDetailAsync(invoice, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Invalid", ex.Message));
        }
    }

    public async Task<Result<InvoiceDto>> UpdateDraftInvoiceAsync(
        Guid organizationId,
        Guid userId,
        Guid invoiceId,
        DateOnly dueDate,
        string? notes,
        IReadOnlyList<InvoiceItemLineDto> items,
        CancellationToken cancellationToken = default)
    {
        if (items.Count == 0)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Items", "At least one line item is required."));
        }

        var existing = await invoiceRepository.GetByIdAsync(invoiceId, organizationId, cancellationToken);
        if (existing is null)
        {
            return Result<InvoiceDto>.Failure(Error.NotFound("Finance.Invoice.NotFound", "Invoice not found."));
        }

        if (!InvoiceRules.CanEdit(existing.Status))
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.State", "Only draft invoices can be edited."));
        }

        if (items.Any(i => i.Quantity < 0 || i.UnitPrice < 0))
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Items", "Line quantities and unit prices must be non-negative."));
        }

        var total = items.Sum(i => Math.Round(i.Quantity * i.UnitPrice, 2, MidpointRounding.AwayFromZero));

        if (total <= 0)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Total", "Total amount must be greater than zero."));
        }

        try
        {
            existing.ReplaceDraftTotals(dueDate, notes, total);
            await invoiceRepository.UpdateAsync(existing, cancellationToken);
            await invoiceItemRepository.SoftDeleteByInvoiceIdAsync(invoiceId, userId, cancellationToken);

            var lineEntities = items
                .Select(line => InvoiceItemEntity.Create(
                    invoiceId,
                    line.ItemType,
                    line.Description,
                    line.Quantity,
                    line.UnitPrice,
                    line.ServiceId,
                    line.MeterReadingId,
                    line.TicketLogId))
                .ToList();

            await invoiceItemRepository.AddRangeAsync(lineEntities, cancellationToken);
            return Result<InvoiceDto>.Success(await MapInvoiceDetailAsync(existing, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Invalid", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.State", ex.Message));
        }
    }

    public async Task<Result<InvoiceDto>> PublishInvoiceAsync(
        Guid organizationId,
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, organizationId, cancellationToken);
        if (invoice is null)
        {
            return Result<InvoiceDto>.Failure(Error.NotFound("Finance.Invoice.NotFound", "Invoice not found."));
        }

        var lines = await invoiceItemRepository.ListActiveByInvoiceIdAsync(invoiceId, cancellationToken);
        if (lines.Count == 0)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Items", "Invoice must have at least one line to publish."));
        }

        try
        {
            invoice.Publish();
        }
        catch (InvalidOperationException ex)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.State", ex.Message));
        }

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);

        if (invoice.LeaseId is { } lid)
        {
            var lease = await leaseReadGateway.GetLeaseAsync(lid, organizationId, cancellationToken);
            if (lease?.PrimaryResidentUserId is { } uid && uid != Guid.Empty)
            {
                await integrationEventPublisher.PublishAsync(
                    new NotificationRequestedEvent(
                        uid,
                        $"Invoice #{invoice.InvoiceNo}",
                        $"You have a new invoice for {invoice.TotalAmount} due on {invoice.DueDate:yyyy-MM-dd}.",
                        "Info",
                        null),
                    cancellationToken);
            }
        }

        return Result<InvoiceDto>.Success(await MapInvoiceDetailAsync(invoice, cancellationToken));
    }

    public async Task<Result<InvoiceDto>> CancelInvoiceAsync(
        Guid organizationId,
        Guid userId,
        Guid invoiceId,
        string? reason,
        CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, organizationId, cancellationToken);
        if (invoice is null)
        {
            return Result<InvoiceDto>.Failure(Error.NotFound("Finance.Invoice.NotFound", "Invoice not found."));
        }

        var previous = invoice.Status;
        try
        {
            invoice.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Invoice.Cancel", ex.Message));
        }

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);

        if ((previous.Equals(InvoiceStatuses.Sent, StringComparison.OrdinalIgnoreCase)
             || previous.Equals(InvoiceStatuses.Overdue, StringComparison.OrdinalIgnoreCase))
            && invoice.LeaseId is { } lid)
        {
            var lease = await leaseReadGateway.GetLeaseAsync(lid, organizationId, cancellationToken);
            if (lease?.PrimaryResidentUserId is { } uid && uid != Guid.Empty)
            {
                var msg = string.IsNullOrWhiteSpace(reason)
                    ? $"Invoice #{invoice.InvoiceNo} has been cancelled."
                    : $"Invoice #{invoice.InvoiceNo} has been cancelled. Reason: {reason}";
                await integrationEventPublisher.PublishAsync(
                    new NotificationRequestedEvent(uid, "Invoice cancelled", msg, "Warning", null),
                    cancellationToken);
            }
        }

        return Result<InvoiceDto>.Success(await MapInvoiceDetailAsync(invoice, cancellationToken));
    }

    public async Task<Result<InvoiceDto>> GetInvoiceAsync(
        Guid organizationId,
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, organizationId, cancellationToken);
        if (invoice is null)
        {
            return Result<InvoiceDto>.Failure(Error.NotFound("Finance.Invoice.NotFound", "Invoice not found."));
        }

        return Result<InvoiceDto>.Success(await MapInvoiceDetailAsync(invoice, cancellationToken));
    }

    public async Task<Result<InvoiceDto>> GetInvoiceForTenantAsync(
        Guid tenantUserId,
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
        if (invoice is null || string.Equals(invoice.Status, InvoiceStatuses.Draft, StringComparison.OrdinalIgnoreCase))
        {
            return Result<InvoiceDto>.Failure(Error.NotFound("Finance.Invoice.NotFound", "Invoice not found."));
        }

        var leaseIds = await leaseReadGateway.GetLeaseIdsForResidentUserAsync(tenantUserId, cancellationToken);
        if (invoice.LeaseId is null || !leaseIds.Contains(invoice.LeaseId.Value))
        {
            return Result<InvoiceDto>.Failure(Error.Forbidden("Finance.Invoice.Forbidden", "You cannot access this invoice."));
        }

        return Result<InvoiceDto>.Success(await MapInvoiceDetailAsync(invoice, cancellationToken));
    }

    public async Task<Result<PagedInvoicesDto>> SearchInvoicesAsync(
        Guid organizationId,
        IReadOnlyList<string>? statuses,
        Guid? leaseId,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? search,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        var query = new InvoiceSearchQuery(organizationId, statuses, leaseId, fromDate, toDate, search, page, perPage);
        var (rows, total) = await invoiceRepository.SearchAsync(query, cancellationToken);
        var items = new List<InvoiceDto>();
        foreach (var row in rows)
        {
            items.Add(await MapInvoiceListRowAsync(row, cancellationToken));
        }

        return Result<PagedInvoicesDto>.Success(new PagedInvoicesDto(items, total, page, perPage));
    }

    public async Task<Result<IReadOnlyList<InvoiceDto>>> ListTenantInvoicesAsync(
        Guid tenantUserId,
        CancellationToken cancellationToken = default)
    {
        var leaseIds = await leaseReadGateway.GetLeaseIdsForResidentUserAsync(tenantUserId, cancellationToken);
        var invoices = await invoiceRepository.ListForLeaseIdsAsync(leaseIds, cancellationToken);
        var list = new List<InvoiceDto>();
        foreach (var inv in invoices)
        {
            list.Add(MapInvoiceList(inv));
        }

        return Result<IReadOnlyList<InvoiceDto>>.Success(list);
    }

    public async Task<Result<InvoiceDto>> RecordManualPaymentAsync(
        RecordManualPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.MethodId == Guid.Empty)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Payment.Method", "Payment method is required."));
        }

        var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, request.OrganizationId, cancellationToken);
        if (invoice is null)
        {
            return Result<InvoiceDto>.Failure(Error.NotFound("Finance.Invoice.NotFound", "Invoice not found."));
        }

        if (!InvoiceRules.CanRecordPayment(invoice.Status))
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Payment.State", "Cannot record payment for this invoice status."));
        }

        var remaining = invoice.TotalAmount - invoice.PaidAmount;
        if (request.Amount <= 0 || request.Amount > remaining)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Payment.Amount", "Amount must be positive and not greater than balance due."));
        }

        var paidAt = request.PaidAtUtc.Kind == DateTimeKind.Utc
            ? request.PaidAtUtc
            : request.PaidAtUtc.ToUniversalTime();

        if (paidAt > DateTime.UtcNow.AddDays(1))
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Payment.Date", "Paid date cannot be far in the future."));
        }

        var payload = string.IsNullOrWhiteSpace(request.Note)
            ? null
            : JsonSerializer.Serialize(new { note = request.Note });

        var payment = PaymentEntity.CreateSuccess(
            invoice.Id,
            request.MethodId,
            request.Amount,
            paidAt,
            request.ReferenceCode,
            payload);

        if (!string.IsNullOrWhiteSpace(request.ReferenceCode)
            && await paymentRepository.ExistsByReferenceCodeAsync(request.ReferenceCode.Trim(), cancellationToken))
        {
            return Result<InvoiceDto>.Failure(Error.Conflict("Finance.Payment.Duplicate", "Reference code already used."));
        }

        try
        {
            invoice.ApplyPayment(request.Amount);
        }
        catch (InvalidOperationException ex)
        {
            return Result<InvoiceDto>.Failure(Error.BadRequest("Finance.Payment.Apply", ex.Message));
        }

        await paymentRepository.AddAsync(payment, cancellationToken);
        await invoiceRepository.UpdateAsync(invoice, cancellationToken);

        if (string.Equals(invoice.Status, InvoiceStatuses.Paid, StringComparison.OrdinalIgnoreCase)
            && invoice.LeaseId is { } lid)
        {
            var lease = await leaseReadGateway.GetLeaseAsync(lid, request.OrganizationId, cancellationToken);
            if (lease?.PrimaryResidentUserId is { } uid && uid != Guid.Empty)
            {
                await integrationEventPublisher.PublishAsync(
                    new NotificationRequestedEvent(
                        uid,
                        "Payment received",
                        $"Invoice #{invoice.InvoiceNo} is paid in full.",
                        "Info",
                        null),
                    cancellationToken);
            }
        }

        return Result<InvoiceDto>.Success(await MapInvoiceDetailAsync(invoice, cancellationToken));
    }

    public async Task<Result<PagedPaymentsDto>> SearchPaymentsAsync(
        Guid organizationId,
        DateTime? fromPaidAtUtc,
        DateTime? toPaidAtUtc,
        Guid? methodId,
        string? status,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        var q = new PaymentSearchQuery(organizationId, fromPaidAtUtc, toPaidAtUtc, methodId, status, page, perPage);
        var (rows, total) = await paymentRepository.SearchAsync(q, cancellationToken);
        var dtos = rows.Select(r => MapPaymentRow(r)).ToList();
        return Result<PagedPaymentsDto>.Success(new PagedPaymentsDto(dtos, total, page, perPage));
    }

    public async Task<Result<IReadOnlyList<PaymentDto>>> ListTenantPaymentsAsync(
        Guid tenantUserId,
        CancellationToken cancellationToken = default)
    {
        var leaseIds = await leaseReadGateway.GetLeaseIdsForResidentUserAsync(tenantUserId, cancellationToken);
        var payments = await paymentRepository.ListSuccessByLeaseIdsAsync(leaseIds, cancellationToken);
        var list = new List<PaymentDto>();
        foreach (var p in payments)
        {
            list.Add(await MapPaymentAsync(p, cancellationToken));
        }

        return Result<IReadOnlyList<PaymentDto>>.Success(list);
    }

    public Task<Result<OnlinePaymentInitiationResult>> InitiateOnlinePaymentAsync(
        Guid tenantUserId,
        Guid invoiceId,
        string methodKey,
        CancellationToken cancellationToken = default) =>
        onlinePaymentInitiator.InitiateAsync(invoiceId, tenantUserId, methodKey, cancellationToken);

    public Task<Result> HandlePaymentWebhookAsync(
        string rawBody,
        IReadOnlyDictionary<string, string> headers,
        CancellationToken cancellationToken = default) =>
        paymentWebhookHandler.HandleAsync(rawBody, headers, cancellationToken);

    public async Task<Result<DepositRefundDto>> CreateDepositRefundAsync(
        CreateDepositRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await depositRefundRepository.HasPendingOrApprovedForLeaseAsync(request.LeaseId, cancellationToken))
        {
            return Result<DepositRefundDto>.Failure(
                Error.Conflict("Finance.DepositRefund.Pending", "A deposit refund is already pending for this lease."));
        }

        var lease = await leaseReadGateway.GetLeaseAsync(request.LeaseId, request.OrganizationId, cancellationToken);
        if (lease is null)
        {
            return Result<DepositRefundDto>.Failure(Error.NotFound("Finance.Lease.NotFound", "Lease was not found."));
        }

        try
        {
            var entity = DepositRefundEntity.CreatePending(
                request.LeaseId,
                request.OrganizationId,
                lease.PrimaryResidentUserId,
                request.Amount,
                request.Notes,
                request.UserId);

            await depositRefundRepository.AddAsync(entity, cancellationToken);

            if (lease.PrimaryResidentUserId is { } uid && uid != Guid.Empty)
            {
                await integrationEventPublisher.PublishAsync(
                    new NotificationRequestedEvent(
                        uid,
                        "Deposit refund requested",
                        $"A deposit refund of {request.Amount} has been requested.",
                        "Info",
                        null),
                    cancellationToken);
            }

            return Result<DepositRefundDto>.Success(MapDeposit(entity));
        }
        catch (ArgumentException ex)
        {
            return Result<DepositRefundDto>.Failure(Error.BadRequest("Finance.DepositRefund.Invalid", ex.Message));
        }
    }

    public async Task<Result<DepositRefundDto>> ConfirmDepositRefundPaidAsync(
        Guid organizationId,
        Guid refundId,
        ConfirmDepositRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await depositRefundRepository.GetByIdAsync(refundId, organizationId, cancellationToken);
        if (entity is null)
        {
            return Result<DepositRefundDto>.Failure(Error.NotFound("Finance.DepositRefund.NotFound", "Refund not found."));
        }

        try
        {
            var paidAt = request.PaidAtUtc.Kind == DateTimeKind.Utc
                ? request.PaidAtUtc
                : request.PaidAtUtc.ToUniversalTime();
            entity.MarkPaid(request.UserId, paidAt);
            await depositRefundRepository.UpdateAsync(entity, cancellationToken);

            if (entity.TenantId is { } tid && tid != Guid.Empty)
            {
                await integrationEventPublisher.PublishAsync(
                    new NotificationRequestedEvent(
                        tid,
                        "Deposit refunded",
                        $"{entity.Amount} has been refunded.",
                        "Info",
                        null),
                    cancellationToken);
            }

            return Result<DepositRefundDto>.Success(MapDeposit(entity));
        }
        catch (InvalidOperationException ex)
        {
            return Result<DepositRefundDto>.Failure(Error.BadRequest("Finance.DepositRefund.State", ex.Message));
        }
    }

    public async Task<Result<DepositRefundDto>> ForfeitDepositRefundAsync(
        Guid organizationId,
        Guid refundId,
        ForfeitDepositRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await depositRefundRepository.GetByIdAsync(refundId, organizationId, cancellationToken);
        if (entity is null)
        {
            return Result<DepositRefundDto>.Failure(Error.NotFound("Finance.DepositRefund.NotFound", "Refund not found."));
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return Result<DepositRefundDto>.Failure(Error.BadRequest("Finance.DepositRefund.Reason", "Reason is required."));
        }

        try
        {
            entity.Forfeit(request.Reason);
            await depositRefundRepository.UpdateAsync(entity, cancellationToken);

            if (entity.TenantId is { } tid && tid != Guid.Empty)
            {
                await integrationEventPublisher.PublishAsync(
                    new NotificationRequestedEvent(
                        tid,
                        "Deposit refund forfeited",
                        request.Reason,
                        "Warning",
                        null),
                    cancellationToken);
            }

            return Result<DepositRefundDto>.Success(MapDeposit(entity));
        }
        catch (InvalidOperationException ex)
        {
            return Result<DepositRefundDto>.Failure(Error.BadRequest("Finance.DepositRefund.State", ex.Message));
        }
    }

    public Task<Result<int>> RunAutoInvoiceGenerationAsync(DateOnly runDate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Auto invoice generation stub for {RunDate}. Connect Lease + meter services to implement UC-F02.",
            runDate);
        return Task.FromResult(Result<int>.Success(0));
    }

    public async Task<Result<int>> RunOverdueSweepAsync(DateOnly asOfDate, CancellationToken cancellationToken = default)
    {
        var due = await invoiceRepository.ListSentDueBeforeAsync(asOfDate, cancellationToken);
        var count = 0;
        foreach (var inv in due)
        {
            try
            {
                inv.MarkOverdue();
                await invoiceRepository.UpdateAsync(inv, cancellationToken);
                count++;

                if (inv.LeaseId is { } lid)
                {
                    var lease = await leaseReadGateway.GetLeaseAsync(lid, inv.OrganizationId, cancellationToken);
                    if (lease?.PrimaryResidentUserId is { } uid && uid != Guid.Empty)
                    {
                        await integrationEventPublisher.PublishAsync(
                            new NotificationRequestedEvent(
                                uid,
                                $"Invoice #{inv.InvoiceNo} overdue",
                                $"Balance due: {inv.TotalAmount - inv.PaidAmount}.",
                                "Warning",
                                null),
                            cancellationToken);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Domain rule prevented marking invoice {InvoiceId} overdue", inv.Id);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Infrastructure failure marking invoice {InvoiceId} overdue — aborting sweep", inv.Id);
                throw;
            }
        }

        return Result<int>.Success(count);
    }

    public async Task<Result<IReadOnlyList<RevenueMonthDto>>> GetRevenueByMonthAsync(
        Guid organizationId,
        int year,
        CancellationToken cancellationToken = default)
    {
        var rows = await paymentRepository.GetMonthlyRevenueForYearAsync(organizationId, year, cancellationToken);
        var list = rows
            .Select(r => new RevenueMonthDto(new DateOnly(year, r.Month, 1), r.TotalRevenue, r.InvoiceCount, r.LeaseCount))
            .ToList();
        return Result<IReadOnlyList<RevenueMonthDto>>.Success(list);
    }

    public async Task<Result<DebtSummaryDto>> GetDebtSummaryAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var asOf = DateOnly.FromDateTime(DateTime.UtcNow);
        var data = await invoiceRepository.GetDebtSummaryAsync(organizationId, asOf, cancellationToken);
        return Result<DebtSummaryDto>.Success(
            new DebtSummaryDto(data.OverdueCount, data.OverdueAmount, data.DueSoonCount, data.DueSoonAmount));
    }

    private async Task<InvoiceDto> MapInvoiceDetailAsync(InvoiceEntity invoice, CancellationToken cancellationToken)
    {
        var items = await invoiceItemRepository.ListActiveByInvoiceIdAsync(invoice.Id, cancellationToken);
        var payments = await paymentRepository.ListByInvoiceIdAsync(invoice.Id, cancellationToken);
        var paymentDtos = new List<PaymentDto>();
        foreach (var p in payments)
        {
            paymentDtos.Add(await MapPaymentAsync(p, cancellationToken));
        }

        return new InvoiceDto(
            invoice.Id,
            invoice.OrganizationId,
            invoice.LeaseId,
            invoice.IsAutoCreated,
            invoice.InvoiceNo,
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.Status,
            invoice.TotalAmount,
            invoice.PaidAmount,
            invoice.Notes,
            invoice.CreatedBy,
            invoice.CreatedAt,
            invoice.UpdatedAt,
            LeaseNo: null,
            TenantName: null,
            items.Select(MapItem).ToList(),
            paymentDtos);
    }

    private Task<InvoiceDto> MapInvoiceListRowAsync(InvoiceListRow row, CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        var dto = MapInvoiceList(row.Invoice);
        return Task.FromResult(dto with { LeaseNo = row.LeaseNo, TenantName = row.TenantName });
    }

    private static InvoiceDto MapInvoiceList(InvoiceEntity invoice) =>
            new InvoiceDto(
                invoice.Id,
                invoice.OrganizationId,
                invoice.LeaseId,
                invoice.IsAutoCreated,
                invoice.InvoiceNo,
                invoice.InvoiceDate,
                invoice.DueDate,
                invoice.Status,
                invoice.TotalAmount,
                invoice.PaidAmount,
                invoice.Notes,
                invoice.CreatedBy,
                invoice.CreatedAt,
                invoice.UpdatedAt,
                LeaseNo: null,
                TenantName: null,
                Array.Empty<InvoiceItemDto>(),
                Array.Empty<PaymentDto>());

    private static InvoiceItemDto MapItem(InvoiceItemEntity x) =>
        new(
            x.Id,
            x.ItemType,
            x.Description,
            x.Quantity,
            x.UnitPrice,
            x.Amount,
            x.ServiceId,
            x.MeterReadingId,
            x.TicketLogId);

    private Task<PaymentDto> MapPaymentAsync(PaymentEntity p, CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return Task.FromResult(
            new PaymentDto(
                p.Id,
                p.InvoiceId,
                p.MethodId,
                p.Amount,
                p.PaidAt,
                p.ReferenceCode,
                p.Status,
                MethodName: null));
    }

    private static PaymentDto MapPaymentRow(PaymentListRow row) =>
        new(
            row.Payment.Id,
            row.Payment.InvoiceId,
            row.Payment.MethodId,
            row.Payment.Amount,
            row.Payment.PaidAt,
            row.Payment.ReferenceCode,
            row.Payment.Status,
            row.MethodName);

    private static DepositRefundDto MapDeposit(DepositRefundEntity e) =>
        new(e.Id, e.LeaseId, e.OrganizationId, e.TenantId, e.Amount, e.Status, e.Notes, e.CreatedAt, e.PaidAt);
}
