using Finance.Domain.Events;
using RoomManagerment.Messaging.Contracts.Events;

namespace Finance.Infrastructure.Services;

internal static class FinanceIntegrationEventMapper
{
    private const string ExchangeName = "finance.events";

    public static bool TryMap(Finance.Domain.Common.IDomainEvent domainEvent, out IntegrationEventEnvelope? envelope)
    {
        envelope = domainEvent switch
        {
            InvoiceCreatedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(InvoiceCreatedIntegrationEvent),
                new InvoiceCreatedIntegrationEvent
                {
                    InvoiceId = e.InvoiceId,
                    OrganizationId = e.OrganizationId,
                    LeaseId = e.LeaseId,
                    InvoiceNo = e.InvoiceNo,
                    TotalAmount = e.TotalAmount,
                    DueDate = e.DueDate,
                    IsAutoCreated = e.IsAutoCreated,
                    CreatedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Finance"
                }),

            InvoicePublishedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(InvoicePublishedIntegrationEvent),
                new InvoicePublishedIntegrationEvent
                {
                    InvoiceId = e.InvoiceId,
                    OrganizationId = e.OrganizationId,
                    LeaseId = e.LeaseId,
                    InvoiceNo = e.InvoiceNo,
                    TotalAmount = e.TotalAmount,
                    DueDate = e.DueDate,
                    PublishedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Finance"
                }),

            InvoiceCancelledEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(InvoiceCancelledIntegrationEvent),
                new InvoiceCancelledIntegrationEvent
                {
                    InvoiceId = e.InvoiceId,
                    OrganizationId = e.OrganizationId,
                    LeaseId = e.LeaseId,
                    InvoiceNo = e.InvoiceNo,
                    Reason = e.Reason,
                    CancelledAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Finance"
                }),

            InvoicePaymentRecordedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(InvoicePaymentRecordedIntegrationEvent),
                new InvoicePaymentRecordedIntegrationEvent
                {
                    InvoiceId = e.InvoiceId,
                    OrganizationId = e.OrganizationId,
                    PaymentId = e.PaymentId,
                    Amount = e.Amount,
                    NewPaidAmount = e.NewPaidAmount,
                    TotalAmount = e.TotalAmount,
                    IsFullyPaid = e.IsFullyPaid,
                    ReferenceCode = e.ReferenceCode,
                    RecordedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Finance"
                }),

            InvoiceOverdueEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(InvoiceOverdueIntegrationEvent),
                new InvoiceOverdueIntegrationEvent
                {
                    InvoiceId = e.InvoiceId,
                    OrganizationId = e.OrganizationId,
                    LeaseId = e.LeaseId,
                    InvoiceNo = e.InvoiceNo,
                    DueDate = e.DueDate,
                    OutstandingAmount = e.OutstandingAmount,
                    MarkedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Finance"
                }),

            DepositRefundCreatedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(DepositRefundCreatedIntegrationEvent),
                new DepositRefundCreatedIntegrationEvent
                {
                    RefundId = e.RefundId,
                    OrganizationId = e.OrganizationId,
                    LeaseId = e.LeaseId,
                    TenantId = e.TenantId,
                    Amount = e.Amount,
                    CreatedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Finance"
                }),

            DepositRefundPaidEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(DepositRefundPaidIntegrationEvent),
                new DepositRefundPaidIntegrationEvent
                {
                    RefundId = e.RefundId,
                    OrganizationId = e.OrganizationId,
                    LeaseId = e.LeaseId,
                    TenantId = e.TenantId,
                    Amount = e.Amount,
                    PaidAt = e.PaidAtUtc,
                    EventCreatedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Finance"
                }),

            DepositRefundForfeitedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(DepositRefundForfeitedIntegrationEvent),
                new DepositRefundForfeitedIntegrationEvent
                {
                    RefundId = e.RefundId,
                    OrganizationId = e.OrganizationId,
                    LeaseId = e.LeaseId,
                    TenantId = e.TenantId,
                    Reason = e.Reason,
                    ForfeitedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Finance"
                }),

            NotificationRequestedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(NotificationCreateRequestedEvent),
                new NotificationCreateRequestedEvent
                {
                    RecipientUserId = e.RecipientUserId,
                    Title = e.Title,
                    Message = e.Message,
                    Type = e.Type,
                    SourceService = "Finance",
                    CreatedAt = e.OccurredOn.UtcDateTime,
                    Metadata = e.Metadata is null ? null : new Dictionary<string, string>(e.Metadata)
                }),

            _ => null
        };

        return envelope is not null;
    }
}
