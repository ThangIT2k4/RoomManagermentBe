using System.Collections.Concurrent;
using System.Reflection;
using Identity.Domain.Common;
using Identity.Domain.Exceptions;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;

namespace Identity.Infrastructure;

internal static class QueryFilterBuilder
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, EntityField2>> FieldCache = new();

    public static PredicateExpression? Build<TFields>(QueryFilter? filter)
        => Build(filter, typeof(TFields));

    public static PredicateExpression? Build(QueryFilter? filter, Type fieldsType)
    {
        if (filter is null || filter.Conditions.Count == 0)
            return null;

        var fields = GetFields(fieldsType);
        var useOr = string.Equals(filter.Logic, "or", StringComparison.OrdinalIgnoreCase);

        var expression = new PredicateExpression();

        foreach (var condition in filter.Conditions)
        {
            if (!fields.TryGetValue(condition.Field, out var field))
                throw new QueryFilterException($"Unknown field '{condition.Field}'.");

            var predicate = BuildPredicate(field, condition);

            if (useOr)
                expression.AddWithOr(predicate);
            else
                expression.Add(predicate);
        }

        return expression;
    }

    private static IReadOnlyDictionary<string, EntityField2> GetFields(Type fieldsType)
    {
        return FieldCache.GetOrAdd(fieldsType, type =>
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

            var dict = new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase);

            foreach (var prop in properties)
            {
                if (prop.PropertyType != typeof(EntityField2))
                    continue;

                if (prop.GetValue(null) is EntityField2 field)
                    dict[prop.Name] = field;
            }

            if (dict.Count == 0)
                throw new QueryFilterException($"No EntityField2 found on {type.Name}");

            return dict;
        });
    }

    private static IPredicate BuildPredicate(EntityField2 field, FilterCondition condition)
    {
        var op = NormalizeOperator(condition.Operator);

        return op switch
        {
            FilterOperator.Eq => field.Equal(condition.Value),
            FilterOperator.Neq => field.NotEqual(condition.Value),
            FilterOperator.Gt => field.GreaterThan(condition.Value),
            FilterOperator.Gte => field.GreaterEqual(condition.Value),
            FilterOperator.Lt => field.LesserThan(condition.Value),
            FilterOperator.Lte => field.LesserEqual(condition.Value),

            FilterOperator.Contains => BuildLike(field, condition.Value, LikeMode.Contains),
            FilterOperator.StartsWith => BuildLike(field, condition.Value, LikeMode.StartsWith),
            FilterOperator.EndsWith => BuildLike(field, condition.Value, LikeMode.EndsWith),
            FilterOperator.Like => BuildLike(field, condition.Value, LikeMode.Raw),

            FilterOperator.In => BuildIn(field, condition.Values, false),
            FilterOperator.NotIn => BuildIn(field, condition.Values, true),

            FilterOperator.Between => BuildBetween(field, condition.Values, false),
            FilterOperator.NotBetween => BuildBetween(field, condition.Values, true),

            FilterOperator.IsNull => field.IsNull(),
            FilterOperator.IsNotNull => field.IsNotNull(),

            _ => throw new QueryFilterException($"Unsupported operator '{condition.Operator}'.")
        };
    }

    private static IPredicate BuildLike(EntityField2 field, object? value, LikeMode mode)
    {
        if (value is not string str)
            throw new QueryFilterException("LIKE operators require string value.");

        return mode switch
        {
            LikeMode.Contains => field.Contains(str),
            LikeMode.StartsWith => field.StartsWith(str),
            LikeMode.EndsWith => field.EndsWith(str),
            _ => field.Like(str)
        };
    }

    private static IPredicate BuildIn(EntityField2 field, IReadOnlyList<object?>? values, bool negate)
    {
        if (values is null || values.Count == 0)
            throw new QueryFilterException("IN operator requires values.");

        if (values.Count > 200)
            throw new QueryFilterException("Too many values for IN operator.");

        return negate ? field.NotIn(values.ToArray()) : field.In(values.ToArray());
    }

    private static IPredicate BuildBetween(EntityField2 field, IReadOnlyList<object?>? values, bool negate)
    {
        if (values is null || values.Count != 2)
            throw new QueryFilterException("BETWEEN requires exactly two values.");

        return negate
            ? field.NotBetween(values[0], values[1])
            : field.Between(values[0], values[1]);
    }

    private static FilterOperator NormalizeOperator(string op)
    {
        return op.Trim().ToLowerInvariant() switch
        {
            "=" or "==" or "eq" => FilterOperator.Eq,
            "!=" or "<>" or "neq" => FilterOperator.Neq,
            ">" or "gt" => FilterOperator.Gt,
            ">=" or "gte" => FilterOperator.Gte,
            "<" or "lt" => FilterOperator.Lt,
            "<=" or "lte" => FilterOperator.Lte,

            "contains" => FilterOperator.Contains,
            "startswith" => FilterOperator.StartsWith,
            "endswith" => FilterOperator.EndsWith,
            "like" => FilterOperator.Like,

            "in" => FilterOperator.In,
            "notin" or "nin" => FilterOperator.NotIn,

            "between" => FilterOperator.Between,
            "notbetween" => FilterOperator.NotBetween,

            "isnull" => FilterOperator.IsNull,
            "isnotnull" => FilterOperator.IsNotNull,

            _ => throw new QueryFilterException($"Invalid operator '{op}'.")
        };
    }

    private enum LikeMode
    {
        Raw,
        Contains,
        StartsWith,
        EndsWith
    }

    private enum FilterOperator
    {
        Eq,
        Neq,
        Gt,
        Gte,
        Lt,
        Lte,
        Contains,
        StartsWith,
        EndsWith,
        Like,
        In,
        NotIn,
        Between,
        NotBetween,
        IsNull,
        IsNotNull
    }
}
