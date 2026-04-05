namespace Auth.Domain.Common;

public class DomainException : Exception
{
    public string? ErrorCode { get; }

    protected DomainException() { }

    protected DomainException(string message) : base(message) { }

    protected DomainException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}   