using CRM.Domain.Entities;

namespace CRM.Domain.Repositories;

public interface ILeadRepository
{
    Task<LeadEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LeadEntity> AddAsync(LeadEntity lead, CancellationToken cancellationToken = default);
    Task<LeadEntity> UpdateAsync(LeadEntity lead, CancellationToken cancellationToken = default);
}

