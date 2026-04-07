using Lease.Application.Services;
using MassTransit;
using RoomManagerment.Messaging.Contracts.Messages;

namespace Lease.Infrastructure.Consumers;

public sealed class LeaseExpiringCheckRequestedConsumer(ILeaseApplicationService service) : IConsumer<LeaseExpiringCheckRequested>
{
    public async Task Consume(ConsumeContext<LeaseExpiringCheckRequested> context)
    {
        await service.RunExpiringLeaseCheckAsync(context.Message.AsOfDate, context.CancellationToken);
    }
}

public sealed class LeaseExpirySweepRequestedConsumer(ILeaseApplicationService service) : IConsumer<LeaseExpirySweepRequested>
{
    public async Task Consume(ConsumeContext<LeaseExpirySweepRequested> context)
    {
        await service.RunLeaseExpirySweepAsync(context.Message.AsOfDate, context.CancellationToken);
    }
}
