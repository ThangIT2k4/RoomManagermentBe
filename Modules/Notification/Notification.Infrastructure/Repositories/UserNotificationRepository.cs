using Notification.Domain.Entities;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Mapper;
using RoomManagerment.Notification.DatabaseSpecific;
using RoomManagerment.Notification.FactoryClasses;
using RoomManagerment.Notification.HelperClasses;
using RoomManagerment.Notification.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace Notification.Infrastructure.Repositories;

public sealed class UserNotificationRepository(DataAccessAdapter adapter) : IUserNotificationRepository
{
    public async Task<UserNotificationEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserNotification
            .Where(un => un.Id == id)
            .FirstOrDefaultAsync(ct);
        if (dal is null) return null;
        NotificationEntity? notif = null;
        if (dal.Notification != null)
            notif = dal.Notification.ToDomain();
        return dal.ToDomain(notif);
    }

    public async Task<UserNotificationEntity?> GetByUserAndNotificationAsync(Guid userId, Guid notificationId, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserNotification
            .Where(un => un.UserId == userId && un.NotificationId == notificationId)
            .FirstOrDefaultAsync(ct);
        if (dal is null) return null;
        NotificationEntity? notif = null;
        if (dal.Notification != null)
            notif = dal.Notification.ToDomain();
        return dal.ToDomain(notif);
    }

    public async Task<PagedResult<UserNotificationEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, bool? isRead, CancellationToken ct = default)
    {
        var qf = new QueryFactory();
        var baseQuery = qf.UserNotification.Where(UserNotificationFields.UserId.Equal(userId));
        if (isRead.HasValue)
            baseQuery = baseQuery.Where(UserNotificationFields.IsRead.Equal(isRead.Value));

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery
            .OrderBy(UserNotificationFields.Id.Descending())
            .Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var list = data.Cast<RoomManagerment.Notification.EntityClasses.UserNotificationEntity>().ToList();

        var notificationIds = list.Select(un => un.NotificationId).Distinct().ToList();
        var notifDict = new Dictionary<Guid, NotificationEntity>();
        if (notificationIds.Count > 0)
        {
            var linq = new LinqMetaData(adapter);
            var notifications = await linq.Notification
                .Where(n => notificationIds.Contains(n.Id))
                .ToListAsync(ct);
            foreach (var n in notifications)
                notifDict[n.Id] = n.ToDomain();
        }

        var entities = list
            .Select(dal => dal.ToDomain(notifDict.GetValueOrDefault(dal.NotificationId)))
            .ToList();

        return new PagedResult<UserNotificationEntity>(entities, totalCount, page, pageSize);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        var qf = new QueryFactory();
        var query = qf.UserNotification
            .Where(UserNotificationFields.UserId.Equal(userId))
            .Where(UserNotificationFields.IsRead.Equal(false));
        var count = await adapter.FetchScalarAsync<int>(query.Select(Functions.CountRow()), ct);
        return count;
    }

    public async Task<UserNotificationEntity> AddAsync(UserNotificationEntity userNotification, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Notification.EntityClasses.UserNotificationEntity
        {
            Id = userNotification.Id,
            UserId = userNotification.UserId,
            NotificationId = userNotification.NotificationId,
            IsRead = userNotification.IsRead,
            ReadAt = userNotification.ReadAt
        };
        await adapter.SaveEntityAsync(dal, true, false, ct);
        return userNotification;
    }

    public async Task<bool> MarkAsReadAsync(Guid userNotificationId, Guid userId, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserNotification
            .Where(un => un.Id == userNotificationId && un.UserId == userId)
            .FirstOrDefaultAsync(ct) as RoomManagerment.Notification.EntityClasses.UserNotificationEntity;
        if (dal is null) return false;
        if (dal.IsRead) return true;
        dal.IsRead = true;
        dal.ReadAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(dal, true, false, ct);
        return true;
    }
}
