using Auth.Domain.Common;

namespace Auth.Domain.Exceptions;

public sealed class InvalidEntityIdException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "ENTITY_ID_EMPTY";
    public const string CodeInvalid = "ENTITY_ID_INVALID";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Entity identifier cannot be empty.",
        CodeInvalid => "Entity identifier is invalid.",
        _ => "Entity identifier validation failed."
    };
}

