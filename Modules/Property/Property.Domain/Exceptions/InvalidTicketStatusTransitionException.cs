using Property.Domain.Common;

namespace Property.Domain.Exceptions;

public sealed class InvalidTicketStatusTransitionException(string message) : DomainException(message);
