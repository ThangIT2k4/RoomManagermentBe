namespace CRM.Domain.Common;

public sealed class QueryFilter
{
    public List<FilterCondition> Conditions { get; init; } = [];
    public string Logic { get; init; } = "and";
}
