namespace CRM.Domain.Common;

public sealed class FilterCondition
{
    public required string Field { get; init; }
    public required string Operator { get; init; }

    // Dùng cho eq, gt, like...
    public object? Value { get; init; }

    // Dùng cho in, between
    public IReadOnlyList<object?>? Values { get; init; }
}