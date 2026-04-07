namespace Organization.Domain.ValueObjects;

public sealed record BankAccountNumber
{
    public string Value { get; }

    private BankAccountNumber(string value)
    {
        Value = value;
    }

    public static BankAccountNumber Create(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length is < 6 or > 20 || normalized.Any(ch => !char.IsDigit(ch)))
        {
            throw new ArgumentException("Account number must be 6-20 digits.", nameof(value));
        }

        return new BankAccountNumber(normalized);
    }
}
