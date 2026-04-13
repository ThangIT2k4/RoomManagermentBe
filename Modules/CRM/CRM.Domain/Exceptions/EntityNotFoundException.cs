namespace CRM.Domain.Exceptions;

public sealed class EntityNotFoundException : CrmDomainException
{
    public EntityNotFoundException(string entityName, Guid id)
        : base("crm.entity.not_found", $"Không tìm thấy {entityName} với id '{id}'.")
    {
    }
}
