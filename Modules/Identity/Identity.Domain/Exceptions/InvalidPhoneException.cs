using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class InvalidPhoneException(string errorCode)
    : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "PHONE_EMPTY";
    public const string CodeInvalidLength = "PHONE_INVALID_LENGTH";
    public const string CodeInvalid = "PHONE_INVALID";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Phone cannot be empty.",
        CodeInvalidLength => "Phone length is invalid.",
        CodeInvalid => "Phone format is invalid.",
        _ => "Phone validation failed."
    };
}
