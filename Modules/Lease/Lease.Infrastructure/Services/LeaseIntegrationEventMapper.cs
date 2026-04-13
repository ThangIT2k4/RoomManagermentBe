using Lease.Domain.Common;
using Lease.Domain.Events;
using RoomManagerment.Messaging.Contracts.Events;

namespace Lease.Infrastructure.Services;

internal static class LeaseIntegrationEventMapper
{
    private const string ExchangeName = "lease.events";

    public static bool TryMap(IDomainEvent domainEvent, out IntegrationEventEnvelope? envelope)
    {
        envelope = domainEvent switch
        {
            LeaseActivatedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(LeaseActivatedIntegrationEvent),
                new LeaseActivatedIntegrationEvent
                {
                    LeaseId = e.LeaseId,
                    UnitId = e.UnitId,
                    OrganizationId = e.OrganizationId,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    RentAmount = e.RentAmount,
                    DepositAmount = e.DepositAmount,
                    ActivatedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Lease"
                }),

            LeaseUpdatedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(LeaseUpdatedIntegrationEvent),
                new LeaseUpdatedIntegrationEvent
                {
                    LeaseId = e.LeaseId,
                    UnitId = e.UnitId,
                    ChangedFields = e.ChangedFields,
                    UpdatedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Lease"
                }),

            LeaseRenewedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(LeaseRenewedIntegrationEvent),
                new LeaseRenewedIntegrationEvent
                {
                    OldLeaseId = e.OldLeaseId,
                    NewLeaseId = e.NewLeaseId,
                    UnitId = e.UnitId,
                    RentAmount = e.RentAmount,
                    StartDate = e.StartDate,
                    RenewedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Lease"
                }),

            LeaseTerminatedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(LeaseTerminatedIntegrationEvent),
                new LeaseTerminatedIntegrationEvent
                {
                    LeaseId = e.LeaseId,
                    UnitId = e.UnitId,
                    OrganizationId = e.OrganizationId,
                    TerminationDate = e.TerminationDate,
                    Reason = e.Reason,
                    TerminatedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Lease"
                }),

            LeaseExpiredEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(LeaseExpiredIntegrationEvent),
                new LeaseExpiredIntegrationEvent
                {
                    LeaseId = e.LeaseId,
                    UnitId = e.UnitId,
                    OrganizationId = e.OrganizationId,
                    ExpiredAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Lease"
                }),

            ResidentLinkedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(ResidentLinkedIntegrationEvent),
                new ResidentLinkedIntegrationEvent
                {
                    LeaseId = e.LeaseId,
                    ResidentId = e.ResidentId,
                    UserId = e.UserId,
                    LinkedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Lease"
                }),

            LeaseServiceSetAppliedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(LeaseServiceSetAppliedIntegrationEvent),
                new LeaseServiceSetAppliedIntegrationEvent
                {
                    LeaseId = e.LeaseId,
                    UnitId = e.UnitId,
                    OrganizationId = e.OrganizationId,
                    LeaseServiceSetId = e.LeaseServiceSetId,
                    AppliedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Lease"
                }),

            _ => null
        };

        return envelope is not null;
    }
}
