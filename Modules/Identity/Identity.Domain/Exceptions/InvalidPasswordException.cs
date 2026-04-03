using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class InvalidPasswordException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    // mật khẩu ít nhất 8 ký tự
    // phải bao gồm ít nhất 1 ký tự đặc biệt, 1 chữ hoa, 1 chữ thường và 1 số
    public const string CodeEmpty = "PASSWORD_EMPTY";
    public const string CodeTooShort = "PASSWORD_TOO_SHORT";
    public const string CodeNoSpecialCharacter = "PASSWORD_NO_SPECIAL_CHARACTER";
    public const string CodeNoNumber = "PASSWORD_NO_NUMBER";
    public const string CodeNoUppercase = "PASSWORD_NO_UPPERCASE";
    public const string CodeNoLowercase = "PASSWORD_NO_LOWERCASE";
    public const string CodeNoWhitespace = "PASSWORD_NO_WHITESPACE";
    public const string CodeTooLong = "PASSWORD_TOO_LONG";
    public const string CodeInvalid = "PASSWORD_INVALID";
    
    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Password cannot be empty.",
        CodeTooLong => "Password cannot be longer than 255 characters.",
        CodeInvalid => "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.",
        _ => "Invalid password."
    };
}