namespace Auth.Domain.Common;

public class DomainException : Exception
{
    public string? ErrorCode { get; }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}