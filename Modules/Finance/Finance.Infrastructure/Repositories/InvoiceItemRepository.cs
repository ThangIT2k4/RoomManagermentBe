using Finance.Domain.Entities;
using Finance.Domain.Repositories;
using Finance.Infrastructure.Mapper;
using RoomManagerment.Finance.DatabaseSpecific;
using RoomManagerment.Finance.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using DalInvoiceItem = RoomManagerment.Finance.EntityClasses.InvoiceItemEntity;

namespace Finance.Infrastructure.Repositories;

public sealed class InvoiceItemRepository(DataAccessAdapter adapter) : IInvoiceItemRepository
{
    public async Task<IReadOnlyList<InvoiceItemEntity>> ListActiveByInvoiceIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var list = await linq.InvoiceItem
            .Where(x => x.InvoiceId == invoiceId && x.DeletedAt == null)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return list.Select(x => x.ToDomain()).ToList();
    }

    public async Task AddRangeAsync(IEnumerable<InvoiceItemEntity> items, CancellationToken cancellationToken = default)
    {
        foreach (var item in items)
        {
            var dal = ToDal(item);
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        }
    }

    public async Task SoftDeleteByInvoiceIdAsync(Guid invoiceId, Guid deletedBy, CancellationToken cancellationToken = default)
    {
        var linq = new LinqMetaData(adapter);
        var list = await linq.InvoiceItem
            .Where(x => x.InvoiceId == invoiceId && x.DeletedAt == null)
            .ToListAsync(cancellationToken);

        var utc = DateTime.UtcNow;
        foreach (var dal in list)
        {
            dal.DeletedAt = utc;
            dal.DeletedBy = deletedBy;
            dal.UpdatedAt = utc;
            await adapter.SaveEntityAsync(dal, true, false, cancellationToken);
        }
    }

    private static DalInvoiceItem ToDal(InvoiceItemEntity item)
    {
        return new DalInvoiceItem
        {
            Id = item.Id,
            InvoiceId = item.InvoiceId,
            ItemType = item.ItemType,
            Description = item.Description,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Amount = item.Amount,
            ServiceId = item.ServiceId,
            MeterReadingId = item.MeterReadingId,
            TicketLogId = item.TicketLogId,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            DeletedAt = item.DeletedAt,
            DeletedBy = item.DeletedBy
        };
    }
}
