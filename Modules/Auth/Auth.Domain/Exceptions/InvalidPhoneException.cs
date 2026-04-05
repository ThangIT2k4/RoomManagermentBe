using Auth.Domain.Common;

namespace Auth.Domain.Exceptions;

public sealed class InvalidPhoneException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "PHONE_EMPTY";
    public const string CodeTooLong = "PHONE_TOO_LONG";
    public const string CodeInvalid = "PHONE_INVALID";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Phone cannot be empty.",
        CodeTooLong => "Phone exceeds maximum length.",
        CodeInvalid => "Phone format is invalid.",
        _ => "Phone validation failed."
    };
}

