using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class InvalidPermissionException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "PERMISSION_CODE_EMPTY";
    public const string CodeTooLong = "PERMISSION_CODE_TOO_LONG";
    public const string NameEmpty = "PERMISSION_NAME_EMPTY";
    public const string NameTooLong = "PERMISSION_NAME_TOO_LONG";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Permission code cannot be empty.",
        CodeTooLong => "Permission code exceeds maximum length.",
        NameEmpty => "Permission name cannot be empty.",
        NameTooLong => "Permission name exceeds maximum length.",
        _ => "Permission validation failed."
    };
}
