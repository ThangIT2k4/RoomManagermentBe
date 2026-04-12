using Finance.Application.Dtos;
using Finance.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Payments.SearchPayments;

public sealed class SearchPaymentsQueryHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<SearchPaymentsQuery, Result<PagedPaymentsDto>>
{
    public Task<Result<PagedPaymentsDto>> Handle(SearchPaymentsQuery request, CancellationToken cancellationToken)
        => finance.SearchPaymentsAsync(
            request.OrganizationId,
            request.FromPaidAtUtc,
            request.ToPaidAtUtc,
            request.MethodId,
            request.Status,
            request.Page,
            request.PerPage,
            cancellationToken);
}

public sealed class ListTenantPaymentsQueryHandler(IFinanceApplicationService finance)
    : IAppRequestHandler<ListTenantPaymentsQuery, Result<IReadOnlyList<PaymentDto>>>
{
    public Task<Result<IReadOnlyList<PaymentDto>>> Handle(ListTenantPaymentsQuery request, CancellationToken cancellationToken)
        => finance.ListTenantPaymentsAsync(request.TenantUserId, cancellationToken);
}
