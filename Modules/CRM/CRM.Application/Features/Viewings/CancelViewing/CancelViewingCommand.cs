using CRM.Application.Features.Viewings;
using MediatR;

namespace CRM.Application.Features.Viewings.CancelViewing;

/// <summary>Command: hủy viewing (ghi).</summary>
public sealed record CancelViewingCommand(Guid ViewingId, string Reason, Guid RequestedBy)
    : IRequest<Result<ViewingEntityDto>>;
