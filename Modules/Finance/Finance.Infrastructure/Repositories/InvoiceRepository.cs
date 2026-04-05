using Finance.Domain.Entities;
using Finance.Domain.Repositories;
using Finance.Infrastructure.Mapper;
using RoomManagerment.Finance.DatabaseSpecific;
using RoomManagerment.Finance.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalInvoice = RoomManagerment.Finance.EntityClasses.InvoiceEntity;

namespace Finance.Infrastructure.Repositories;

public sealed class InvoiceRepository(DataAccessAdapter adapter) : IInvoiceRepository
{
    public async Task<InvoiceEntity?> GetByIdAsync(Guid id, Guid organizationId, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Invoice
            .Where(x => x.Id == id && x.OrganizationId == organizationId && x.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task<InvoiceEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Invoice.Where(x => x.Id == id && x.DeletedAt == null).FirstOrDefaultAsync(cancellationToken);
        return dal?.ToDomain();
    }

    public async Task AddAsync(InvoiceEntity invoice, CancellationToken cancellationToken = default)
    {
        var dal = ToDal(invoice);
        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
    }

    public async Task UpdateAsync(InvoiceEntity invoice, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Invoice.Where(x => x.Id == invoice.Id).FirstOrDefaultAsync(cancellationToken)
                  ?? throw new InvalidOperationException("Invoice not found for update.");

        dal.OrganizationId = invoice.OrganizationId;
        dal.LeaseId = invoice.LeaseId;
        dal.IsAutoCreated = invoice.IsAutoCreated;
        dal.InvoiceNo = invoice.InvoiceNo;
        dal.InvoiceDate = invoice.InvoiceDate;
        dal.DueDate = invoice.DueDate;
        dal.Status = invoice.Status;
        dal.TotalAmount = invoice.TotalAmount;
        dal.PaidAmount = invoice.PaidAmount;
        dal.Notes = invoice.Notes;
        dal.UpdatedAt = invoice.UpdatedAt;

        await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
    }

    public async Task<(IReadOnlyList<InvoiceListRow> Rows, int TotalCount)> SearchAsync(
        InvoiceSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var q = linq.Invoice.Where(x => x.OrganizationId == query.OrganizationId && x.DeletedAt == null);

        if (query.Statuses is { Count: > 0 })
        {
            var set = query.Statuses.ToHashSet(StringComparer.OrdinalIgnoreCase);
            q = q.Where(x => set.Contains(x.Status!));
        }

        if (query.LeaseId is { } leaseId)
        {
            q = q.Where(x => x.LeaseId == leaseId);
        }

        if (query.FromDate is { } from)
        {
            q = q.Where(x => x.InvoiceDate >= from);
        }

        if (query.ToDate is { } to)
        {
            q = q.Where(x => x.InvoiceDate <= to);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim();
            q = q.Where(x => x.InvoiceNo != null && x.InvoiceNo.Contains(s));
        }

        var total = await q.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var per = Math.Clamp(query.PerPage, 1, 200);
        var skip = (page - 1) * per;

        var list = await q
            .OrderByDescending(x => x.InvoiceDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Take(per)
            .ToListAsync(cancellationToken);

        var rows = list
            .Select(x => new InvoiceListRow(x.ToDomain(), LeaseNo: null, TenantName: null))
            .ToList();

        return (rows, total);
    }

    public async Task<int> GetMaxInvoiceSequenceForYearAsync(
        Guid organizationId,
        int year,
        CancellationToken cancellationToken = default)
    {
        var prefix = $"INV{year}-";
        var linq = new LinqMetaData(adapter);
        var nos = await linq.Invoice
            .Where(x => x.OrganizationId == organizationId && x.InvoiceNo != null && x.InvoiceNo.StartsWith(prefix))
            .Select(x => x.InvoiceNo!)
            .ToListAsync(cancellationToken);

        var max = 0;
        foreach (var n in nos)
        {
            if (n.Length <= prefix.Length)
            {
                continue;
            }

            var suffix = n[prefix.Length..];
            if (int.TryParse(suffix, out var v) && v > max)
            {
                max = v;
            }
        }

        return max;
    }

    public async Task<IReadOnlyList<InvoiceEntity>> ListSentDueBeforeAsync(
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var list = await linq.Invoice
            .Where(x => x.DeletedAt == null && x.Status == Finance.Domain.InvoiceStatuses.Sent && x.DueDate < asOfDate)
            .ToListAsync(cancellationToken);
        return list.Select(x => x.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<InvoiceEntity>> ListForLeaseIdsAsync(
        IReadOnlyList<Guid> leaseIds,
        CancellationToken cancellationToken = default)
    {
        if (leaseIds.Count == 0)
        {
            return Array.Empty<InvoiceEntity>();
        }

        var idSet = leaseIds.ToHashSet();
        var linq = new LinqMetaData(adapter);
        var list = await linq.Invoice
            .Where(x =>
                x.DeletedAt == null
                && x.LeaseId != null
                && idSet.Contains(x.LeaseId.Value)
                && (x.Status == Finance.Domain.InvoiceStatuses.Sent
                    || x.Status == Finance.Domain.InvoiceStatuses.Overdue
                    || x.Status == Finance.Domain.InvoiceStatuses.Paid))
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync(cancellationToken);
        return list.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DebtSummaryData> GetDebtSummaryAsync(
        Guid organizationId,
        DateOnly asOf,
        CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var baseInvoices = linq.Invoice.Where(x =>
            x.OrganizationId == organizationId
            && x.DeletedAt == null
            && x.Status != Finance.Domain.InvoiceStatuses.Paid
            && x.Status != Finance.Domain.InvoiceStatuses.Cancelled
            && x.Status != Finance.Domain.InvoiceStatuses.Draft);

        var overdueQuery = baseInvoices.Where(x =>
            x.Status == Finance.Domain.InvoiceStatuses.Overdue
            || (x.Status == Finance.Domain.InvoiceStatuses.Sent && x.DueDate < asOf));

        var overdueList = await overdueQuery.ToListAsync(cancellationToken);
        var overdueCount = overdueList.Count;
        var overdueAmount = overdueList.Sum(x => x.TotalAmount - x.PaidAmount);

        var dueSoonCutoff = asOf.AddDays(7);
        var dueSoonQuery = baseInvoices.Where(x =>
            x.Status == Finance.Domain.InvoiceStatuses.Sent
            && x.DueDate >= asOf
            && x.DueDate <= dueSoonCutoff);

        var dueSoonList = await dueSoonQuery.ToListAsync(cancellationToken);
        var dueSoonCount = dueSoonList.Count;
        var dueSoonAmount = dueSoonList.Sum(x => x.TotalAmount - x.PaidAmount);

        return new DebtSummaryData(overdueCount, overdueAmount, dueSoonCount, dueSoonAmount);
    }

    private static DalInvoice ToDal(InvoiceEntity invoice)
    {
        return new DalInvoice
        {
            Id = invoice.Id,
            OrganizationId = invoice.OrganizationId,
            LeaseId = invoice.LeaseId,
            IsAutoCreated = invoice.IsAutoCreated,
            InvoiceNo = invoice.InvoiceNo,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            Notes = invoice.Notes,
            CreatedBy = invoice.CreatedBy,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt,
            DeletedAt = invoice.DeletedAt,
            DeletedBy = invoice.DeletedBy
        };
    }
}
