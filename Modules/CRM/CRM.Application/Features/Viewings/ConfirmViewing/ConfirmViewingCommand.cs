using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.ConfirmViewing;

public sealed record ConfirmViewingCommand(Guid ViewingId) : IAppRequest<Result<ViewingEntityDto>>;
