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
}

