using Organization.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Settings.UpsertEmailSettings;

public sealed record UpsertEmailSettingsCommand(
    Guid OrganizationId,
    Guid ActorUserId,
    string FromName,
    string FromEmail,
    string Provider,
    string? SmtpHost,
    int? SmtpPort,
    string? SmtpEncryption,
    string? SmtpUsername,
    string? SmtpPassword) : IAppRequest<Result>;
