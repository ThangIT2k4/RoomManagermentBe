using Auth.Domain.Common;

namespace Auth.Domain.Exceptions;

public sealed class InvalidUserStateException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeInvalidStatus = "USER_STATUS_INVALID";
    public const string CodeInvalidTransition = "USER_STATUS_TRANSITION_INVALID";
    public const string CodeInactiveUser = "USER_INACTIVE";
    public const string CodeBannedUser = "USER_BANNED";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeInvalidStatus => "User status is invalid.",
        CodeInvalidTransition => "User status transition is invalid.",
        CodeInactiveUser => "User is inactive.",
        CodeBannedUser => "User is banned.",
        _ => "User state validation failed."
    };
}

