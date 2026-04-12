using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Jobs.RunOverdueSweep;

public sealed class RunOverdueSweepCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<RunOverdueSweepCommand, Result<int>>
{
    public Task<Result<int>> Handle(RunOverdueSweepCommand request, CancellationToken cancellationToken)
        => finance.RunOverdueSweepAsync(request.AsOfDate, cancellationToken);
}
