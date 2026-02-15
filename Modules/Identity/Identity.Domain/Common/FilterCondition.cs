namespace Identity.Domain.Common;

public sealed class FilterCondition
{
    public string Field { get; init; } = default!;
    public string Operator { get; init; } = default!;
    public string Value { get; init; } = default!;
}