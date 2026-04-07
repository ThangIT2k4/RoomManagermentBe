using Lease.Domain.Common;

namespace Lease.Domain.Exceptions;

public sealed class InvalidLeaseStateException : DomainException
{
    public const string CodeInvalidTransition = "Lease.InvalidTransition";
    public const string CodeInvalidStatus = "Lease.InvalidStatus";

    public InvalidLeaseStateException(string code, string message) : base(code, message) { }
}
