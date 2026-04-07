using DalLeaseEntity = RoomManagerment.Lease.EntityClasses.LeaseEntity;
using DomainLeaseEntity = Lease.Domain.Entities.LeaseEntity;

namespace Lease.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static DomainLeaseEntity ToDomain(this DalLeaseEntity dal)
    {
        return DomainLeaseEntity.FromPersistence(
            dal.Id,
            dal.UnitId,
            dal.OrganizationId,
            dal.LeaseNo,
            dal.RentAmount,
            dal.DepositAmount,
            dal.CycleId,
            dal.PaymentDay,
            dal.Status ?? string.Empty,
            dal.StartDate,
            dal.EndDate,
            dal.ParentLeaseId,
            dal.BookingId,
            dal.Notes,
            dal.CreatedAt,
            dal.UpdatedAt);
    }
}

