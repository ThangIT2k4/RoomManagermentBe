using MassTransit;
using Microsoft.Extensions.Logging;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class LeadStatusChangedConsumer(ILogger<LeadStatusChangedConsumer> logger) : IConsumer<LeadStatusChangedIntegrationEvent>
{
    public Task Consume(ConsumeContext<LeadStatusChangedIntegrationEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation(
            "Received LeadStatusChangedIntegrationEvent for lead {LeadId}: {PreviousStatus} -> {CurrentStatus}",
            evt.LeadId,
            evt.PreviousStatus,
            evt.CurrentStatus);
        return Task.CompletedTask;
    }
}
