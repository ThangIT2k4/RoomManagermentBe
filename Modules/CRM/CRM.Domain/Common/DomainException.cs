using System.Runtime.Serialization;

namespace CRM.Domain.Common;

[Serializable]
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

#pragma warning disable SYSLIB0051
    protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        ErrorCode = info.GetString(nameof(ErrorCode));
    }

    [Obsolete("Formatter-based serialization is obsolete.")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ErrorCode), ErrorCode);
    }
#pragma warning restore SYSLIB0051
}   