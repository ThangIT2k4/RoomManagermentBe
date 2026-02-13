using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class InvalidUserProfileException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string UserIdEmpty = "USER_PROFILE_USER_ID_EMPTY";
    public const string FullNameTooLong = "USER_PROFILE_FULL_NAME_TOO_LONG";
    public const string PhoneTooLong = "USER_PROFILE_PHONE_TOO_LONG";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        UserIdEmpty => "UserId cannot be empty.",
        FullNameTooLong => "FullName exceeds maximum length.",
        PhoneTooLong => "Phone exceeds maximum length.",
        _ => "UserProfile validation failed."
    };
}
