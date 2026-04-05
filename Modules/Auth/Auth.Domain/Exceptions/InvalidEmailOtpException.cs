using Auth.Domain.Common;

namespace Auth.Domain.Exceptions;

public sealed class InvalidEmailOtpException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmptyOtp = "EMAIL_OTP_EMPTY";
    public const string CodeInvalidOtp = "EMAIL_OTP_INVALID";
    public const string CodeExpired = "EMAIL_OTP_EXPIRED";
    public const string CodeUsed = "EMAIL_OTP_USED";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmptyOtp => "OTP cannot be empty.",
        CodeInvalidOtp => "OTP is invalid.",
        CodeExpired => "OTP has expired.",
        CodeUsed => "OTP has already been used.",
        _ => "OTP validation failed."
    };
}

