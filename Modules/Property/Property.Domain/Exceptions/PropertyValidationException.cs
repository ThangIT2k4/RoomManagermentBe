using Property.Domain.Common;

namespace Property.Domain.Exceptions;

public sealed class PropertyValidationException(string message) : DomainException(message);
