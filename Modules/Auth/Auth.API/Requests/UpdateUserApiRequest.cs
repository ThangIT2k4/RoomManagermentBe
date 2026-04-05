namespace Auth.API.Requests;

public sealed record UpdateUserApiRequest(string? Email, string? Username, string? Phone, short? Status);
