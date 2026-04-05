using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Repositories;

public interface IRoleRepository
{
    Task<RoleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoleEntity?> GetByKeyCodeAsync(RoleKey keyCode, CancellationToken cancellationToken = default);
    Task<RoleEntity> AddAsync(RoleEntity role, CancellationToken cancellationToken = default);
    Task<RoleEntity> UpdateAsync(RoleEntity role, CancellationToken cancellationToken = default);
}

