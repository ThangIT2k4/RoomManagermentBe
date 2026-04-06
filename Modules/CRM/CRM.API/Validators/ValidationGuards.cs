using System.Text.RegularExpressions;

namespace CRM.API.Validators;

internal static partial class ValidationGuards
{
    private static readonly Regex SqlInjectionRegex = SqlRegex();
    private static readonly Regex ScriptInjectionRegex = ScriptRegex();

    public static bool BeSafeText(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        if (input.Any(char.IsControl))
        {
            return false;
        }

        return !SqlInjectionRegex.IsMatch(input) && !ScriptInjectionRegex.IsMatch(input);
    }

    [GeneratedRegex(@"(\bunion\b\s+\bselect\b|\binsert\b\s+\binto\b|\bupdate\b\s+\w+\s+\bset\b|\bdelete\b\s+\bfrom\b|\bdrop\b\s+\btable\b|\bexec\b\s*\(|\bxp_\w+|--|/\*|\*/)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SqlRegex();

    [GeneratedRegex(@"(<\s*script\b|</\s*script\s*>|javascript:|on\w+\s*=)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ScriptRegex();
}
