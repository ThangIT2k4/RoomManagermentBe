using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Jobs.RunAutoInvoice;

public sealed class RunAutoInvoiceCommandHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<RunAutoInvoiceCommand, Result<int>>
{
    public Task<Result<int>> Handle(RunAutoInvoiceCommand request, CancellationToken cancellationToken)
        => finance.RunAutoInvoiceGenerationAsync(request.RunDate, cancellationToken);
}
