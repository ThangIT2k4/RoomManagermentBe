using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public class InvalidUserStateException (string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeInactive = "USER_INACTIVE";
    public const string CodeLocked = "USER_LOCKED";
    public const string CodeDeleted = "USER_DELETED";
    public const string CodeLockedToActivate = "USER_LOCKED_TO_ACTIVATE";
    public const string CodeActiveToInactive = "USER_ACTIVE_TO_INACTIVE"; 
    public const string CodeInactiveToActivate = "USER_INACTIVE_TO_ACTIVATE";
    public const string CodeInvalid = "USER_INVALID_STATE";


    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeInactive => "User is inactive.",
        CodeLocked => "User is locked.",
        CodeDeleted => "User is deleted.",
        CodeLockedToActivate => "Locked user cannot be activated.",
        CodeInactiveToActivate => "Inactive user cannot be activated.",
        CodeActiveToInactive => "Active user cannot be deactivated.",
        CodeInvalid => "User state is invalid.",
        _ => "User state validation failed."
    };
}