namespace Auth.Domain.Common;

public sealed class FilterCondition
{
    public required string Field { get; init; }
    public required string Operator { get; init; }

    public object? Value { get; init; }

    public IReadOnlyList<object?>? Values { get; init; }
}