namespace CRM.Domain.Exceptions;

public sealed class EntityNotFoundException : CrmDomainException
{
    public EntityNotFoundException(string entityName, Guid id)
        : base("crm.entity.not_found", $"{entityName} with id '{id}' was not found.")
    {
    }
}
