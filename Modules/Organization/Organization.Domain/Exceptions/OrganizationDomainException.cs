using Organization.Domain.Common;

namespace Organization.Domain.Exceptions;

public sealed class OrganizationDomainException(string message) : DomainException(message);
