namespace Lease.Domain.Entities;

public sealed class PaymentCycleEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public string Name { get; }
    public int DurationMonths { get; }
    public int? DayOfMonth { get; }

    private PaymentCycleEntity(Guid id, Guid organizationId, string name, int durationMonths, int? dayOfMonth)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        DurationMonths = durationMonths;
        DayOfMonth = dayOfMonth;
    }

    public static PaymentCycleEntity Create(Guid organizationId, string name, int durationMonths, int? dayOfMonth)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("OrganizationId is required.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (durationMonths <= 0) throw new ArgumentException("DurationMonths must be greater than 0.", nameof(durationMonths));
        if (dayOfMonth is < 1 or > 28) throw new ArgumentException("DayOfMonth must be between 1 and 28.", nameof(dayOfMonth));
        return new PaymentCycleEntity(Guid.NewGuid(), organizationId, name.Trim(), durationMonths, dayOfMonth);
    }
}
