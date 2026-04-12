using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FullName,
    string? Username,
    string? Phone) : IAppRequest<Result<RegisterResult>>;
