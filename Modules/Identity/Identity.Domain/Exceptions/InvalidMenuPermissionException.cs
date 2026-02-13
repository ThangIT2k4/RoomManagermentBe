using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class InvalidMenuPermissionException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string MenuIdEmpty = "MENU_PERMISSION_MENU_ID_EMPTY";
    public const string PermissionCodeEmpty = "MENU_PERMISSION_PERMISSION_CODE_EMPTY";
    public const string PermissionCodeTooLong = "MENU_PERMISSION_PERMISSION_CODE_TOO_LONG";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        MenuIdEmpty => "MenuId cannot be empty.",
        PermissionCodeEmpty => "PermissionCode cannot be empty.",
        PermissionCodeTooLong => "PermissionCode exceeds maximum length.",
        _ => "MenuPermission validation failed."
    };
}
