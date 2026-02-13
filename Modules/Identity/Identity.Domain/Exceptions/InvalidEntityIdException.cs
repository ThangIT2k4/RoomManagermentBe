using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class InvalidEntityIdException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string RoleIdEmpty = "ENTITY_ROLE_ID_EMPTY";
    public const string PermissionIdEmpty = "ENTITY_PERMISSION_ID_EMPTY";
    public const string UserIdEmpty = "ENTITY_USER_ID_EMPTY";
    public const string MenuIdEmpty = "ENTITY_MENU_ID_EMPTY";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        RoleIdEmpty => "RoleId cannot be empty.",
        PermissionIdEmpty => "PermissionId cannot be empty.",
        UserIdEmpty => "UserId cannot be empty.",
        MenuIdEmpty => "MenuId cannot be empty.",
        _ => "Entity id validation failed."
    };
}
