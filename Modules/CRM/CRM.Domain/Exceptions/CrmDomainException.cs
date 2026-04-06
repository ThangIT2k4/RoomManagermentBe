using CRM.Domain.Common;

namespace CRM.Domain.Exceptions;

public class CrmDomainException : DomainException
{
    public CrmDomainException(string message) : base(message)
    {
    }

    public CrmDomainException(string errorCode, string message) : base(errorCode, message)
    {
    }

    public CrmDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
