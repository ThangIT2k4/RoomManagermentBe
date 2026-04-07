using Property.Domain.Common;

namespace Property.Domain.Exceptions;

public sealed class InvalidUnitStatusTransitionException(string message) : DomainException(message);
