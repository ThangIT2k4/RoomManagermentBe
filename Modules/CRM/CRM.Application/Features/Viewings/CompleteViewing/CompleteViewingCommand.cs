using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.CompleteViewing;

public sealed record CompleteViewingCommand(Guid ViewingId, string? Summary = null, Guid? CompletedBy = null)
    : IAppRequest<Result<ViewingEntityDto>>;
