namespace Property.Domain.Entities;

public static class PropertyStatuses
{
    public const short Available = 1;
    public const short Rented = 2;
    public const short Maintenance = 3;
}

public static class TicketStatuses
{
    public const string Open = "open";
    public const string InProgress = "in_progress";
    public const string Resolved = "resolved";
    public const string Closed = "closed";
    public const string Cancelled = "cancelled";
}
