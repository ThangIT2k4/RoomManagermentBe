using Property.Domain.Common;

namespace Property.Domain.ValueObjects;

public sealed record MeterNumber : ValueObject
{
    public string Value { get; }
    private MeterNumber(string value) => Value = value;
    public static MeterNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Meter number is required.", nameof(value));
        return new MeterNumber(value.Trim());
    }
}
