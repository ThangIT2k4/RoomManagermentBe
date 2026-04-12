using RoomManagerment.Shared.Messaging;

namespace CRM.Application.Features.Viewings.CreateViewing;

public sealed record CreateViewingCommand(
    Guid LeadId,
    DateTime ScheduledAt,
    string? Location,
    Guid AgentUserId,
    string? Note = null)
    : IAppRequest<Result<ViewingEntityDto>>;
