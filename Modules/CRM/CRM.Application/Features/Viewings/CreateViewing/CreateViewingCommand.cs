using CRM.Application.Features.Viewings;
using MediatR;

namespace CRM.Application.Features.Viewings.CreateViewing;

/// <summary>Command: tạo lịch xem phòng (ghi).</summary>
public sealed record CreateViewingCommand(
    Guid LeadId,
    DateTime ScheduledAt,
    string? Location,
    Guid AgentUserId,
    string? Note = null)
    : IRequest<Result<ViewingEntityDto>>;
