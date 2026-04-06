using CRM.Domain.Exceptions;

namespace CRM.Domain.Common;

internal static class EnumValueParser
{
    public static string ParseRequired<TEnum>(string value, string fieldName)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{fieldName} is required.");
        }

        var normalized = value.Trim();
        if (Enum.TryParse<TEnum>(normalized, true, out var parsed))
        {
            return parsed.ToString().ToLowerInvariant();
        }

        var allowed = string.Join(", ", Enum.GetNames<TEnum>().Select(x => x.ToLowerInvariant()));
        throw new DomainValidationException($"Invalid {fieldName}. Allowed values: {allowed}.");
    }

    public static string? ParseOptional<TEnum>(string? value, string fieldName)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return ParseRequired<TEnum>(value, fieldName);
    }
}
