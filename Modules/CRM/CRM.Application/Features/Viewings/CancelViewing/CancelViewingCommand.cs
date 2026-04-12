using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.CancelViewing;

public sealed record CancelViewingCommand(Guid ViewingId, string Reason, Guid RequestedBy)
    : IAppRequest<Result<ViewingEntityDto>>;
