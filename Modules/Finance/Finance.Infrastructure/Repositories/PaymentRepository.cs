using Finance.Domain.Entities;
using Finance.Domain.Repositories;
using Finance.Infrastructure.Mapper;
using RoomManagerment.Finance.DatabaseSpecific;
using RoomManagerment.Finance.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalPayment = RoomManagerment.Finance.EntityClasses.PaymentEntity;

namespace Finance.Infrastructure.Repositories;

public sealed class PaymentRepository(DataAccessAdapter adapter) : IPaymentRepository
{
    public async Task AddAsync(PaymentEntity payment, CancellationToken cancellationToken = default)
    {
        var dal = ToDal(payment);
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentEntity>> ListByInvoiceIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var list = await linq.Payment
            .Where(x => x.InvoiceId == invoiceId && x.DeletedAt == null)
            .OrderByDescending(x => x.PaidAt)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return list.Select(x => x.ToDomain()).ToList();
    }

    public async Task<bool> ExistsByReferenceCodeAsync(string referenceCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(referenceCode))
        {
            return false;
        }

        var linq = new LinqMetaData(adapter);
        return await linq.Payment.AnyAsync(
            x => x.ReferenceCode == referenceCode && x.DeletedAt == null,
            cancellationToken);
    }

    public async Task<(IReadOnlyList<PaymentListRow> Rows, int TotalCount)> SearchAsync(
        PaymentSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var baseQuery =
            from p in linq.Payment
            join i in linq.Invoice on p.InvoiceId equals i.Id
            where i.OrganizationId == query.OrganizationId && p.DeletedAt == null && i.DeletedAt == null
            select new { p, i };

        if (query.FromPaidAtUtc is { } from)
        {
            baseQuery = baseQuery.Where(x => x.p.PaidAt != null && x.p.PaidAt >= from);
        }

        if (query.ToPaidAtUtc is { } to)
        {
            baseQuery = baseQuery.Where(x => x.p.PaidAt != null && x.p.PaidAt <= to);
        }

        if (query.MethodId is { } mid)
        {
            baseQuery = baseQuery.Where(x => x.p.MethodId == mid);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var st = query.Status.Trim();
            baseQuery = baseQuery.Where(x => x.p.Status == st);
        }

        var total = await baseQuery.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var per = Math.Clamp(query.PerPage, 1, 200);
        var skip = (page - 1) * per;

        var pageItems = await baseQuery
            .OrderByDescending(x => x.p.PaidAt)
            .ThenByDescending(x => x.p.CreatedAt)
            .Skip(skip)
            .Take(per)
            .ToListAsync(cancellationToken);

        var methodNames = await ResolveMethodNamesAsync(pageItems.Select(x => x.p.MethodId).Distinct(), cancellationToken);

        var rows = pageItems
            .Select(x => new PaymentListRow(
                x.p.ToDomain(),
                x.i.InvoiceNo,
                x.i.TotalAmount,
                LeaseNo: null,
                TenantName: null,
                x.p.MethodId is { } mid2 && methodNames.TryGetValue(mid2, out var mn) ? mn : null))
            .ToList();

        return (rows, total);
    }

    public async Task<IReadOnlyList<PaymentEntity>> ListSuccessByLeaseIdsAsync(
        IReadOnlyList<Guid> leaseIds,
        CancellationToken cancellationToken = default)
    {
        if (leaseIds.Count == 0)
        {
            return Array.Empty<PaymentEntity>();
        }

        var set = leaseIds.ToHashSet();
        var linq = new LinqMetaData(adapter);
        var joined =
            from p in linq.Payment
            join i in linq.Invoice on p.InvoiceId equals i.Id
            where p.DeletedAt == null
                  && i.DeletedAt == null
                  && p.Status == "success"
                  && i.LeaseId != null
                  && set.Contains(i.LeaseId.Value)
            orderby p.PaidAt descending
            select p;

        var list = await joined.ToListAsync(cancellationToken);
        return list.Select(x => x.ToDomain()).ToList();
    }

    private async Task<Dictionary<Guid, string>> ResolveMethodNamesAsync(
        IEnumerable<Guid?> methodIds,
        CancellationToken cancellationToken)
    {
        var ids = methodIds.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var linq = new LinqMetaData(adapter);
        var methods = await linq.PaymentMethod.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
        return methods.ToDictionary(x => x.Id, x => x.Name ?? string.Empty);
    }

    public async Task<IReadOnlyList<MonthlyRevenueAggregate>> GetMonthlyRevenueForYearAsync(
        Guid organizationId,
        int year,
        CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var rows = await (
                from p in linq.Payment
                join i in linq.Invoice on p.InvoiceId equals i.Id
                where i.OrganizationId == organizationId
                      && p.DeletedAt == null
                      && i.DeletedAt == null
                      && p.Status == "success"
                      && p.PaidAt != null
                      && p.PaidAt.Value.Year == year
                select new { p.Amount, InvoiceId = i.Id, i.LeaseId, Month = p.PaidAt!.Value.Month })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => x.Month)
            .OrderBy(g => g.Key)
            .Select(g => new MonthlyRevenueAggregate(
                g.Key,
                g.Sum(x => x.Amount),
                g.Select(x => x.InvoiceId).Distinct().Count(),
                g.Select(x => x.LeaseId).Where(x => x != null).Select(x => x!.Value).Distinct().Count()))
            .ToList();
    }

    private static DalPayment ToDal(PaymentEntity payment)
    {
        return new DalPayment
        {
            Id = payment.Id,
            InvoiceId = payment.InvoiceId,
            MethodId = payment.MethodId,
            Amount = payment.Amount,
            PaidAt = payment.PaidAt,
            ReferenceCode = payment.ReferenceCode,
            Status = payment.Status,
            RawPayload = payment.RawPayload,
            ErrorMessage = payment.ErrorMessage,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            DeletedAt = payment.DeletedAt,
            DeletedBy = payment.DeletedBy
        };
    }
}
