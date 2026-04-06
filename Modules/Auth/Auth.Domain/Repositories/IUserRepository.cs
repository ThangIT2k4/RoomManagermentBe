using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);
    Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task<UserEntity> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task<PagedResult<UserEntity>> SearchPagedAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 10,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    Task<long> CountAsync(
        string? searchTerm = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        Email email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByUsernameAsync(
        Username username,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default);

    Task SoftDeleteAsync(
        Guid userId,
        Guid deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default);
}
