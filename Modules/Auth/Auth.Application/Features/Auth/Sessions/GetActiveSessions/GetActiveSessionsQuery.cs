using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Sessions.GetActiveSessions;

public sealed record GetActiveSessionsQuery(Guid UserId, int PageNumber = 1, int PageSize = 20)
    : IAppRequest<Result<PagedSessionsResult>>;
