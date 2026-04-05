using Finance.Application.Common;
using Finance.Application.Services;

namespace Finance.Infrastructure.Services;

public sealed class NoOpFinanceScheduledJobPublisher : IFinanceScheduledJobPublisher
{
    public Task<Result> PublishAutoInvoiceGenerationAsync(DateOnly runDate, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success());

    public Task<Result> PublishOverdueSweepAsync(DateOnly asOfDate, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success());
}
