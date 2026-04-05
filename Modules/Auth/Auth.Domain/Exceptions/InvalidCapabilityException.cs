using Auth.Domain.Common;

namespace Auth.Domain.Exceptions;

public sealed class InvalidCapabilityException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "CAPABILITY_KEY_EMPTY";
    public const string CodeInvalid = "CAPABILITY_KEY_INVALID";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Capability key cannot be empty.",
        CodeInvalid => "Capability key is invalid.",
        _ => "Capability validation failed."
    };
}

