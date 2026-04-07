namespace Lease.Domain.Entities;

public sealed class LeaseResidentEntity
{
    public Guid Id { get; }
    public Guid LeaseId { get; }
    public Guid? UserId { get; private set; }
    public string FullName { get; }
    public string? Phone { get; }
    public string? Email { get; }
    public string? IdNumber { get; }
    public string Relationship { get; }
    public bool IsPrimary { get; private set; }

    private LeaseResidentEntity(Guid id, Guid leaseId, Guid? userId, string fullName, string? phone, string? email, string? idNumber, string relationship, bool isPrimary)
    {
        Id = id;
        LeaseId = leaseId;
        UserId = userId;
        FullName = fullName;
        Phone = phone;
        Email = email;
        IdNumber = idNumber;
        Relationship = relationship;
        IsPrimary = isPrimary;
    }

    public static LeaseResidentEntity Create(Guid leaseId, string fullName, string relationship, bool isPrimary, Guid? userId = null, string? phone = null, string? email = null, string? idNumber = null)
    {
        if (leaseId == Guid.Empty) throw new ArgumentException("LeaseId is required.", nameof(leaseId));
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("FullName is required.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(relationship)) throw new ArgumentException("Relationship is required.", nameof(relationship));

        return new LeaseResidentEntity(Guid.NewGuid(), leaseId, userId, fullName.Trim(), phone?.Trim(), email?.Trim(), idNumber?.Trim(), relationship.Trim().ToLowerInvariant(), isPrimary);
    }

    public void SetPrimary(bool isPrimary) => IsPrimary = isPrimary;
    public void LinkUser(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
        if (UserId.HasValue) throw new InvalidOperationException("Resident already linked.");
        UserId = userId;
    }
}
