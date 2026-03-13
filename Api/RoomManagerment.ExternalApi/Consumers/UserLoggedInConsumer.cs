using MassTransit;
using Microsoft.Extensions.Logging;
using RoomManagerment.Messaging.Contracts.Events;

namespace RoomManagerment.ExternalApi.Consumers;

/// <summary>
/// Nhận UserLoggedInEvent từ RabbitMQ (Identity publish sau khi đăng nhập thành công).
/// Dùng để ghi audit log, phân tích hành vi, phát hiện đăng nhập bất thường, v.v.
/// Queue: external-user-logged-in
/// </summary>
public sealed class UserLoggedInConsumer(
    ILogger<UserLoggedInConsumer> logger) : IConsumer<UserLoggedInEvent>
{
    public Task Consume(ConsumeContext<UserLoggedInEvent> context)
    {
        var evt = context.Message;
        // TODO: Lưu vào audit log DB, gửi alert nếu IP lạ, v.v.
        logger.LogInformation(
            "[AuditLog] User {UserId} ({Username}) logged in from {IpAddress} at {LoggedInAt}",
            evt.UserId, evt.Username, evt.IpAddress, evt.LoggedInAt);

        return Task.CompletedTask;
    }
}
