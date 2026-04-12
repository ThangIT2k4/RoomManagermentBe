namespace Lease.Application.Dtos;

public sealed record LeaseRemoveResidentResponse();
public sealed record LeaseSetPrimaryResidentResponse();
public sealed record LeaseApplyServiceSetResponse();
public sealed record LeaseDeletePaymentCycleResponse();
public sealed record LeaseInternalJobQueuedResponse(bool Queued, DateOnly AsOfDate);
