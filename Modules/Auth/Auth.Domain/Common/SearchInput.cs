using System.Text;

namespace Auth.Domain.Common;

public static class SearchInput
{
    public static string? Normalize(string? rawSearchTerm)
    {
        if (string.IsNullOrWhiteSpace(rawSearchTerm))
        {
            return null;
        }

        var trimmed = rawSearchTerm.Trim();
        if (trimmed.Length > InputSecurityLimits.MaxSearchLength)
        {
            trimmed = trimmed[..InputSecurityLimits.MaxSearchLength];
        }

        var builder = new StringBuilder(trimmed.Length);
        foreach (var ch in trimmed)
        {
            if (!char.IsControl(ch))
            {
                builder.Append(ch);
            }
        }

        var normalized = builder.ToString().Trim();
        return normalized.Length == 0 ? null : normalized;
    }
}

