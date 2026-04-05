using Finance.Application.Common;
using Finance.Application.Services;
using MassTransit;
using RoomManagerment.Messaging.Contracts.Messages;

namespace Finance.Infrastructure.Services;

public sealed class MassTransitFinanceScheduledJobPublisher(IBus bus) : IFinanceScheduledJobPublisher
{
    public async Task<Result> PublishAutoInvoiceGenerationAsync(DateOnly runDate, CancellationToken cancellationToken = default)
    {
        await bus.Publish(
            new FinanceAutoInvoiceGenerationRequested { RunDate = runDate, CorrelationId = Guid.NewGuid() },
            cancellationToken);
        return Result.Success();
    }

    public async Task<Result> PublishOverdueSweepAsync(DateOnly asOfDate, CancellationToken cancellationToken = default)
    {
        await bus.Publish(
            new FinanceOverdueSweepRequested { AsOfDate = asOfDate, CorrelationId = Guid.NewGuid() },
            cancellationToken);
        return Result.Success();
    }
}
