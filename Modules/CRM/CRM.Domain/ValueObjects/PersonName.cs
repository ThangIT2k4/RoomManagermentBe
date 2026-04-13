using CRM.Domain.Common;
using CRM.Domain.Exceptions;

namespace CRM.Domain.ValueObjects;

public sealed class PersonName : ValueObject
{
    public string Value { get; }

    private PersonName(string value)
    {
        Value = value;
    }

    public static PersonName Create(string value, string fieldName = "name")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{fieldName} là bắt buộc.");
        }

        var normalized = value.Trim();
        if (normalized.Length > InputSecurityLimits.MaxLeadNameLength)
        {
            throw new DomainValidationException($"Độ dài {fieldName} không được vượt quá {InputSecurityLimits.MaxLeadNameLength}.");
        }

        return new PersonName(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
