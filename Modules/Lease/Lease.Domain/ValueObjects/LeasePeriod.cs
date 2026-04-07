namespace Lease.Domain.ValueObjects;

public readonly record struct LeasePeriod
{
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }

    private LeasePeriod(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static LeasePeriod Create(DateOnly startDate, DateOnly endDate)
    {
        if (endDate <= startDate)
        {
            throw new ArgumentException("End date must be after start date.");
        }

        return new LeasePeriod(startDate, endDate);
    }
}
