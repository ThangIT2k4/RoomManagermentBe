using Integration.Application.Common;
using Integration.Application.Dtos;
using MediatR;

namespace Integration.Application.Features.Actions.RunProviderAction;

public sealed class RunProviderActionCommand : IRequest<Result<ProviderActionExecutionDto>>
{
    public required Guid ConnectionId { get; init; }
    public required string ActionName { get; init; }
    public required string PayloadJson { get; init; }
}
