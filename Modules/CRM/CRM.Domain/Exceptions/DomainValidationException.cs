namespace CRM.Domain.Exceptions;

public sealed class DomainValidationException : CrmDomainException
{
    public DomainValidationException(string message) : base("crm.validation", message)
    {
    }
}
