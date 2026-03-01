using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class QueryFilterException : DomainException
{
    public const string InvalidFilter = "QUERY_FILTER_INVALID";
    public const string UnknownField = "QUERY_FILTER_UNKNOWN_FIELD";
    public const string UnsupportedOperator = "QUERY_FILTER_UNSUPPORTED_OPERATOR";
    public const string InvalidOperatorUsage = "QUERY_FILTER_INVALID_OPERATOR_USAGE";
    public const string NoFieldsDefined = "QUERY_FILTER_NO_FIELDS_DEFINED";

    public QueryFilterException(string message) : base(InvalidFilter, message) { }

    public QueryFilterException(string errorCode, string message) : base(errorCode, message) { }

    public QueryFilterException(string message, Exception innerException)
        : base(message, innerException) { }
}
