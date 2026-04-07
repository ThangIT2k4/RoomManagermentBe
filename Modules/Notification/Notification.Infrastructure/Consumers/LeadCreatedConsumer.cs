using MassTransit;
using Microsoft.Extensions.Logging;
using RoomManagerment.Messaging.Contracts.Events;

namespace Notification.Infrastructure.Consumers;

public sealed class LeadCreatedConsumer(ILogger<LeadCreatedConsumer> logger) : IConsumer<LeadCreatedIntegrationEvent>
{
    public Task Consume(ConsumeContext<LeadCreatedIntegrationEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation(
            "Received LeadCreatedIntegrationEvent for organization {OrganizationId}, lead {LeadId}, status {Status}",
            evt.OrganizationId,
            evt.LeadId,
            evt.Status);
        return Task.CompletedTask;
    }
}
