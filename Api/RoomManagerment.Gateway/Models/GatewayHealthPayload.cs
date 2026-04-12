namespace RoomManagerment.Gateway.Models;

/// <summary>Payload for GET /api/gateway/health.</summary>
public sealed record GatewayHealthPayload(string Service = "gateway");
