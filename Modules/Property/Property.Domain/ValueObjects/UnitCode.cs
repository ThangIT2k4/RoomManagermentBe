using Property.Domain.Common;

namespace Property.Domain.ValueObjects;

public sealed record UnitCode : ValueObject
{
    public string Value { get; }
    private UnitCode(string value) => Value = value;
    public static UnitCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Unit code is required.", nameof(value));
        return new UnitCode(value.Trim());
    }
}
