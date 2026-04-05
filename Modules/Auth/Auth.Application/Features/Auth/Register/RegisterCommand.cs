using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FullName,
    string? Username,
    string? Phone) : IRequest<Result<RegisterResult>>;
