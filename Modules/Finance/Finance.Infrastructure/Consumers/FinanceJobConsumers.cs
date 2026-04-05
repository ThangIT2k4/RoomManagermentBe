using Finance.Application.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using RoomManagerment.Messaging.Contracts.Messages;

namespace Finance.Infrastructure.Consumers;

public sealed class FinanceAutoInvoiceConsumer(
    IFinanceApplicationService financeService,
    ILogger<FinanceAutoInvoiceConsumer> logger) : IConsumer<FinanceAutoInvoiceGenerationRequested>
{
    public async Task Consume(ConsumeContext<FinanceAutoInvoiceGenerationRequested> context)
    {
        logger.LogInformation(
            "Finance auto-invoice job for {RunDate} correlation {CorrelationId}",
            context.Message.RunDate,
            context.Message.CorrelationId);

        var result = await financeService.RunAutoInvoiceGenerationAsync(context.Message.RunDate, context.CancellationToken);
        if (result.IsFailure)
        {
            logger.LogWarning("Auto invoice job failed: {Error}", result.Error?.Message);
        }
    }
}

public sealed class FinanceOverdueSweepConsumer(
    IFinanceApplicationService financeService,
    ILogger<FinanceOverdueSweepConsumer> logger) : IConsumer<FinanceOverdueSweepRequested>
{
    public async Task Consume(ConsumeContext<FinanceOverdueSweepRequested> context)
    {
        logger.LogInformation(
            "Finance overdue sweep for {AsOf} correlation {CorrelationId}",
            context.Message.AsOfDate,
            context.Message.CorrelationId);

        var result = await financeService.RunOverdueSweepAsync(context.Message.AsOfDate, context.CancellationToken);
        if (result.IsFailure)
        {
            logger.LogWarning("Overdue sweep failed: {Error}", result.Error?.Message);
        }
    }
}
