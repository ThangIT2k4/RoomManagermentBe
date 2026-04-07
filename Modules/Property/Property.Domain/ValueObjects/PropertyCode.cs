using Property.Domain.Common;

namespace Property.Domain.ValueObjects;

public sealed record PropertyCode : ValueObject
{
    public string Value { get; }
    private PropertyCode(string value) => Value = value;
    public static PropertyCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Property code is required.", nameof(value));
        return new PropertyCode(value.Trim());
    }
}
