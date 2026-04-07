using Lease.Domain.Common;

namespace Lease.Domain.Exceptions;

public sealed class LeaseValidationException : DomainException
{
    public LeaseValidationException(string code, string message) : base(code, message) { }
}
