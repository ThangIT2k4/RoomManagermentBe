using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public class InvalidPasswordHashException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "PASSWORD_HASH_EMPTY";
    public const string CodeTooLong = "PASSWORD_HASH_TOO_LONG";
    public const string CodeInvalid = "PASSWORD_HASH_INVALID";


    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Password hash cannot be empty.",
        CodeTooLong => "Password hash exceeds maximum length.",
        CodeInvalid => "Password hash format is invalid.",
        _ => "Password hash validation failed."
    };
}