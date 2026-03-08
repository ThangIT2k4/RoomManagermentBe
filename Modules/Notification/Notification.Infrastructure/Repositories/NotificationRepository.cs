using Notification.Domain.Entities;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Mapper;
using RoomManagerment.Notification.DatabaseSpecific;
using RoomManagerment.Notification.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace Notification.Infrastructure.Repositories;

public sealed class NotificationRepository(DataAccessAdapter adapter) : INotificationRepository
{
    public async Task<NotificationEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Notification.Where(n => n.Id == id).FirstOrDefaultAsync(ct);
        return dal?.ToDomain();
    }

    public async Task<NotificationEntity> AddAsync(NotificationEntity notification, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Notification.EntityClasses.NotificationEntity
        {
            Id = notification.Id,
            Title = notification.Title,
            Content = notification.Content,
            Type = notification.Type,
            CreatedAt = notification.CreatedAt
        };
        await adapter.SaveEntityAsync(dal, true, false, ct);
        return notification;
    }
}
