using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public class InvalidUsernameException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "USERNAME_EMPTY";
    public const string CodeTooLong = "USERNAME_TOO_LONG";
    public const string CodeInvalid = "USERNAME_INVALID";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Username cannot be empty.",
        CodeTooLong => "Username exceeds maximum length.",
        CodeInvalid => "Username format is invalid.",
        _ => "Username validation failed."
    };
}