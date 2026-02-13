using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class InvalidRefreshTokenException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string UserIdEmpty = "REFRESH_TOKEN_USER_ID_EMPTY";
    public const string TokenEmpty = "REFRESH_TOKEN_TOKEN_EMPTY";
    public const string TokenTooLong = "REFRESH_TOKEN_TOKEN_TOO_LONG";
    public const string ExpiresAtNotFuture = "REFRESH_TOKEN_EXPIRES_AT_NOT_FUTURE";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        UserIdEmpty => "UserId cannot be empty.",
        TokenEmpty => "Token cannot be empty.",
        TokenTooLong => "Token exceeds maximum length.",
        ExpiresAtNotFuture => "ExpiresAt must be in the future.",
        _ => "RefreshToken validation failed."
    };
}
