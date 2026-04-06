namespace Auth.Domain.Common;

public static class SearchInput
{
    public static string? Normalize(string? searchTerm) =>
        string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
}
