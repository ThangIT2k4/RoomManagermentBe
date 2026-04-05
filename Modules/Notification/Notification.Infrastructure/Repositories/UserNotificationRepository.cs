using Notification.Domain.Common;
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
        var dal = await linq.Notification
            .Where(un => un.Id == id)
            .FirstOrDefaultAsync(ct);
        if (dal is null) return null;
        return dal.ToUserNotificationDomain();
    }

    public async Task<UserNotificationEntity?> GetByUserAndNotificationAsync(Guid userId, Guid notificationId, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Notification
            .Where(un => un.UserId == userId && un.Id == notificationId)
            .FirstOrDefaultAsync(ct);
        if (dal is null) return null;
        return dal.ToUserNotificationDomain();
    }

    public async Task<PagedResult<UserNotificationEntity>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, bool? isRead, CancellationToken ct = default)
    {
        var qf = new QueryFactory();
        var baseQuery = qf.Notification.Where(NotificationFields.UserId.Equal(userId));
        if (isRead.HasValue)
            baseQuery = baseQuery.Where(NotificationFields.IsRead.Equal(isRead.Value));

        var totalCount = await adapter.FetchScalarAsync<int>(baseQuery.Select(Functions.CountRow()), ct);
        var query = baseQuery
            .OrderBy(NotificationFields.Id.Descending())
            .Page(page, pageSize);
        var data = await adapter.FetchQueryAsync(query, ct);
        var entities = data
            .Cast<RoomManagerment.Notification.EntityClasses.NotificationEntity>()
            .Select(dal => dal.ToUserNotificationDomain())
            .ToList();

        return new PagedResult<UserNotificationEntity>(entities, totalCount, page, pageSize);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        var qf = new QueryFactory();
        var query = qf.Notification
            .Where(NotificationFields.UserId.Equal(userId))
            .Where(NotificationFields.IsRead.Equal(false));
        var count = await adapter.FetchScalarAsync<int>(query.Select(Functions.CountRow()), ct);
        return count;
    }

    public Task<UserNotificationEntity> AddAsync(UserNotificationEntity userNotification, CancellationToken ct = default)
    {
        var dal = new RoomManagerment.Notification.EntityClasses.NotificationEntity
        {
            Id = userNotification.Id,
            UserId = userNotification.UserId,
            Title = userNotification.Title,
            Message = userNotification.Content,
            Type = userNotification.Type ?? "Info",
            CreatedAt = userNotification.CreatedAt,
            IsRead = userNotification.IsRead,
            ReadAt = userNotification.ReadAt
        };

        return SaveAndReturnAsync(dal, userNotification, ct);
    }

    private async Task<UserNotificationEntity> SaveAndReturnAsync(
        RoomManagerment.Notification.EntityClasses.NotificationEntity dal,
        UserNotificationEntity userNotification,
        CancellationToken ct)
    {
        await adapter.SaveEntityAsync(dal, true, false, ct);
        return userNotification;
    }

    public async Task<bool> MarkAsReadAsync(Guid userNotificationId, Guid userId, CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.Notification
            .Where(un => un.Id == userNotificationId && un.UserId == userId)
            .FirstOrDefaultAsync(ct);
        if (dal is null) return false;
        if (dal.IsRead) return true;
        dal.IsRead = true;
        dal.ReadAt = DateTime.UtcNow;
        await adapter.SaveEntityAsync(dal, true, false, ct);
        return true;
    }
}
