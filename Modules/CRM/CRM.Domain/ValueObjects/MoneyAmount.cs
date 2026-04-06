using CRM.Domain.Common;
using CRM.Domain.Exceptions;

namespace CRM.Domain.ValueObjects;

public sealed class MoneyAmount : ValueObject
{
    public decimal Value { get; }

    private MoneyAmount(decimal value)
    {
        Value = value;
    }

    public static MoneyAmount Create(decimal value, bool allowZero = false, string fieldName = "amount")
    {
        if (value < 0)
        {
            throw new DomainValidationException($"{fieldName} cannot be negative.");
        }

        if (!allowZero && value == 0)
        {
            throw new DomainValidationException($"{fieldName} must be greater than 0.");
        }

        return new MoneyAmount(decimal.Round(value, 2, MidpointRounding.AwayFromZero));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("0.##");
}
