using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Users.GetUsers;

public sealed record GetUsersQuery(string? SearchTerm, int PageNumber = 1, int PageSize = 20, bool IncludeDeleted = false)
    : IAppRequest<Result<PagedUsersResult>>;
