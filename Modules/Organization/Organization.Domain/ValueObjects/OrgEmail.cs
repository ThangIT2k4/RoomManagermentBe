namespace Organization.Domain.ValueObjects;

public sealed record OrgEmail
{
    public string Value { get; }

    private OrgEmail(string value)
    {
        Value = value;
    }

    public static OrgEmail Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
        {
            throw new ArgumentException("Invalid email.", nameof(value));
        }

        return new OrgEmail(value.Trim().ToLowerInvariant());
    }
}
