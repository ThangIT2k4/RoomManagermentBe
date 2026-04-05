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

public sealed class UserNotificationPreferenceRepository(DataAccessAdapter adapter) : IUserNotificationPreferenceRepository
{
    public async Task<UserNotificationPreferenceEntity?> GetByUserAndEntityTypeAsync(
        Guid userId,
        string entityType,
        CancellationToken ct = default)
    {
        var linq = new LinqMetaData(adapter);
        var dal = await linq.UserNotificationPreference
            .Where(x => x.UserId == userId && x.EntityType == entityType)
            .FirstOrDefaultAsync(ct);

        return dal?.ToDomain();
    }

    public async Task<UserNotificationPreferenceEntity> UpsertAsync(
        UserNotificationPreferenceEntity preference,
        CancellationToken ct = default)
    {
        var existing = await GetByUserAndEntityTypeAsync(preference.UserId, preference.EntityType, ct);

        if (existing is null)
        {
            var newDal = new RoomManagerment.Notification.EntityClasses.UserNotificationPreferenceEntity
            {
                Id = preference.Id,
                UserId = preference.UserId,
                EntityType = preference.EntityType,
                InAppEnabled = preference.InAppEnabled,
                EmailEnabled = preference.EmailEnabled,
                CreatedAt = preference.CreatedAt,
                UpdatedAt = preference.UpdatedAt
            };

            await adapter.SaveEntityAsync(newDal, true, false, ct);
            return preference;
        }

        var qf = new QueryFactory();
        var query = qf.UserNotificationPreference
            .Where(UserNotificationPreferenceFields.UserId.Equal(preference.UserId))
            .Where(UserNotificationPreferenceFields.EntityType.Equal(preference.EntityType));

        var dal = (await adapter.FetchQueryAsync(query, ct))
            .Cast<RoomManagerment.Notification.EntityClasses.UserNotificationPreferenceEntity>()
            .FirstOrDefault();

        if (dal is null)
            return preference;

        dal.InAppEnabled = preference.InAppEnabled;
        dal.EmailEnabled = preference.EmailEnabled;
        dal.UpdatedAt = preference.UpdatedAt ?? DateTime.UtcNow;

        await adapter.SaveEntityAsync(dal, true, false, ct);

        return UserNotificationPreferenceEntity.FromPersistence(
            dal.Id,
            dal.UserId,
            dal.EntityType,
            dal.InAppEnabled,
            dal.EmailEnabled,
            dal.CreatedAt,
            dal.UpdatedAt);
    }
}


