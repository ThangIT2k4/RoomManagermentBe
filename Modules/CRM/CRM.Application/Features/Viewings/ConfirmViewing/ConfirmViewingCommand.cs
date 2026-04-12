using CRM.Application.Features.Viewings;
using MediatR;

namespace CRM.Application.Features.Viewings.ConfirmViewing;

/// <summary>Command: xác nhận viewing (ghi).</summary>
public sealed record ConfirmViewingCommand(Guid ViewingId) : IRequest<Result<ViewingEntityDto>>;
