using Auth.Domain.Common;

namespace Auth.Domain.Exceptions;

public sealed class InvalidEmailException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "EMAIL_EMPTY";
    public const string CodeTooLong = "EMAIL_TOO_LONG";
    public const string CodeInvalid = "EMAIL_INVALID";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Email cannot be empty.",
        CodeTooLong => "Email exceeds maximum length.",
        CodeInvalid => "Email format is invalid.",
        _ => "Email validation failed."
    };
}

