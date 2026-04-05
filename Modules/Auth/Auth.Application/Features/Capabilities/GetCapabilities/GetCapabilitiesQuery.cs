using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Capabilities.GetCapabilities;

public sealed record GetCapabilitiesQuery(string? SearchTerm = null, int PageNumber = 1, int PageSize = 50)
    : IRequest<Result<PagedCapabilitiesResult>>;
