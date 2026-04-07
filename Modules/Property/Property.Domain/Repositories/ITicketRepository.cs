using RoomManagerment.Property.EntityClasses;

namespace Property.Domain.Repositories;

public interface ITicketRepository
{
    Task<TicketEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TicketEntity> AddAsync(TicketEntity entity, CancellationToken cancellationToken = default);
    Task<TicketEntity> UpdateAsync(TicketEntity entity, CancellationToken cancellationToken = default);
}
