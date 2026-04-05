namespace Auth.Domain.Common;

public static class InputSecurityLimits
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
    public const int MaxSearchLength = 100;
    public const int MaxPageNumber = 10_000;
}
