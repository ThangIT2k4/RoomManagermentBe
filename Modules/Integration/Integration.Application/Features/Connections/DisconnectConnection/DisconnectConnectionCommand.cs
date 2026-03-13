using Integration.Application.Common;
using MediatR;

namespace Integration.Application.Features.Connections.DisconnectConnection;

public sealed record DisconnectConnectionCommand(Guid ConnectionId) : IRequest<Result>;
