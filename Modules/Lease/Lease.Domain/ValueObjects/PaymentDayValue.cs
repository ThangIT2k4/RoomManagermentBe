namespace Lease.Domain.ValueObjects;

public readonly record struct PaymentDayValue
{
    public int Value { get; }

    private PaymentDayValue(int value)
    {
        Value = value;
    }

    public static PaymentDayValue Create(int value)
    {
        if (value is < 1 or > 28)
        {
            throw new ArgumentException("Payment day must be between 1 and 28.", nameof(value));
        }

        return new PaymentDayValue(value);
    }
}
