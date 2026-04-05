using Finance.Application.Common;

namespace Finance.Application.Services;

public interface IFinanceScheduledJobPublisher
{
    Task<Result> PublishAutoInvoiceGenerationAsync(DateOnly runDate, CancellationToken cancellationToken = default);

    Task<Result> PublishOverdueSweepAsync(DateOnly asOfDate, CancellationToken cancellationToken = default);
}
