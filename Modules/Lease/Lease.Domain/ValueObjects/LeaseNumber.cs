namespace Lease.Domain.ValueObjects;

public sealed record LeaseNumber
{
    public string Value { get; }

    private LeaseNumber(string value)
    {
        Value = value;
    }

    public static LeaseNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Lease number is required.", nameof(value));
        }

        return new LeaseNumber(value.Trim().ToUpperInvariant());
    }
}
