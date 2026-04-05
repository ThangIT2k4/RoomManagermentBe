using Auth.Domain.Common;

namespace Auth.Domain.Exceptions;

public sealed class InvalidRoleException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "ROLE_KEY_EMPTY";
    public const string CodeInvalid = "ROLE_KEY_INVALID";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Role key cannot be empty.",
        CodeInvalid => "Role key is invalid.",
        _ => "Role validation failed."
    };
}

