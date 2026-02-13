using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class InvalidRoleException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "ROLE_CODE_EMPTY";
    public const string CodeTooLong = "ROLE_CODE_TOO_LONG";
    public const string NameEmpty = "ROLE_NAME_EMPTY";
    public const string NameTooLong = "ROLE_NAME_TOO_LONG";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Role code cannot be empty.",
        CodeTooLong => "Role code exceeds maximum length.",
        NameEmpty => "Role name cannot be empty.",
        NameTooLong => "Role name exceeds maximum length.",
        _ => "Role validation failed."
    };
}
