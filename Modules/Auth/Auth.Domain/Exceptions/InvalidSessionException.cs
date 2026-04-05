using Auth.Domain.Common;

namespace Auth.Domain.Exceptions;

public sealed class InvalidSessionException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmptyToken = "SESSION_TOKEN_EMPTY";
    public const string CodeInvalidToken = "SESSION_TOKEN_INVALID";
    public const string CodeExpired = "SESSION_EXPIRED";
    public const string CodeRevoked = "SESSION_REVOKED";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmptyToken => "Session token cannot be empty.",
        CodeInvalidToken => "Session token is invalid.",
        CodeExpired => "Session has expired.",
        CodeRevoked => "Session has been revoked.",
        _ => "Session validation failed."
    };
}

