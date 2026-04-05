using Auth.Domain.Entities;

namespace Auth.Domain.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfileEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfileEntity> AddAsync(UserProfileEntity profile, CancellationToken cancellationToken = default);
    Task<UserProfileEntity> UpdateAsync(UserProfileEntity profile, CancellationToken cancellationToken = default);
}

