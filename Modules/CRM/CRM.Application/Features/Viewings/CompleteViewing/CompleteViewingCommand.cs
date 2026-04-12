using CRM.Application.Features.Viewings;
using MediatR;

namespace CRM.Application.Features.Viewings.CompleteViewing;

/// <summary>Command: hoàn thành viewing (ghi).</summary>
public sealed record CompleteViewingCommand(Guid ViewingId, string? Summary = null, Guid? CompletedBy = null)
    : IRequest<Result<ViewingEntityDto>>;
