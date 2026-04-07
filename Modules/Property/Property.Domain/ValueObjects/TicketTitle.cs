using Property.Domain.Common;

namespace Property.Domain.ValueObjects;

public sealed record TicketTitle : ValueObject
{
    public string Value { get; }
    private TicketTitle(string value) => Value = value;
    public static TicketTitle Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Ticket title is required.", nameof(value));
        if (value.Trim().Length > 500) throw new ArgumentException("Ticket title max length is 500.", nameof(value));
        return new TicketTitle(value.Trim());
    }
}
