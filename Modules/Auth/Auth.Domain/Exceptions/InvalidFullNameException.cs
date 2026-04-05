using Auth.Domain.Common;

namespace Auth.Domain.Exceptions;

public sealed class InvalidFullNameException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "FULL_NAME_EMPTY";
    public const string CodeTooLong = "FULL_NAME_TOO_LONG";
    public const string CodeInvalid = "FULL_NAME_INVALID";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Full name cannot be empty.",
        CodeTooLong => "Full name exceeds maximum length.",
        CodeInvalid => "Full name is invalid.",
        _ => "Full name validation failed."
    };
}

