using Auth.Domain.Common;
using Auth.Domain.Events;
using RoomManagerment.Messaging.Contracts.Events;

namespace Auth.Infrastructure.Services;

internal static class AuthIntegrationEventMapper
{
    private const string ExchangeName = "auth.events";

    public static bool TryMap(IDomainEvent domainEvent, out IntegrationEventEnvelope? envelope)
    {
        envelope = domainEvent switch
        {
            UserCreatedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(UserRegisteredEvent),
                new UserRegisteredEvent
                {
                    UserId = e.UserId,
                    Username = e.Username ?? e.Email,
                    Email = e.Email,
                    RegisteredAt = e.OccurredOn.UtcDateTime
                }),

            UserLoginRecordedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(UserLoggedInEvent),
                new UserLoggedInEvent
                {
                    UserId = e.UserId,
                    Username = e.Username,
                    IpAddress = e.IpAddress ?? string.Empty,
                    LoggedInAt = e.OccurredOn.UtcDateTime
                }),

            UserPasswordChangedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(PasswordChangedEvent),
                new PasswordChangedEvent
                {
                    UserId = e.UserId,
                    ChangedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Auth"
                }),

            SessionCreatedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(SessionCreatedIntegrationEvent),
                new SessionCreatedIntegrationEvent
                {
                    SessionId = e.SessionId,
                    UserId = e.UserId,
                    CreatedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Auth"
                }),

            SessionRevokedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(SessionRevokedIntegrationEvent),
                new SessionRevokedIntegrationEvent
                {
                    SessionId = e.SessionId,
                    UserId = e.UserId,
                    RevokedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Auth"
                }),

            UserEmailVerifiedEvent e => new IntegrationEventEnvelope(
                ExchangeName,
                nameof(UserEmailVerifiedIntegrationEvent),
                new UserEmailVerifiedIntegrationEvent
                {
                    UserId = e.UserId,
                    Email = e.Email,
                    VerifiedAt = e.OccurredOn.UtcDateTime,
                    SourceService = "Auth"
                }),

            _ => null
        };

        return envelope is not null;
    }
}
