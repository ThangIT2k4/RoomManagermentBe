using Finance.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Finance.Application.Features.Payments.SearchPayments;

public sealed record SearchPaymentsQuery(
    Guid OrganizationId,
    DateTime? FromPaidAtUtc = null,
    DateTime? ToPaidAtUtc = null,
    Guid? MethodId = null,
    string? Status = null,
    int Page = 1,
    int PerPage = 20)
    : IAppRequest<Result<PagedPaymentsDto>>;

public sealed record ListTenantPaymentsQuery(Guid TenantUserId)
    : IAppRequest<Result<IReadOnlyList<PaymentDto>>>;
