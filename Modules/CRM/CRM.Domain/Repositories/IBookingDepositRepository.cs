using CRM.Domain.Entities;

namespace CRM.Domain.Repositories;

public interface IBookingDepositRepository
{
    Task<BookingDepositEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookingDepositEntity> AddAsync(BookingDepositEntity entity, CancellationToken cancellationToken = default);
    Task<BookingDepositEntity> UpdateAsync(BookingDepositEntity entity, CancellationToken cancellationToken = default);
}
