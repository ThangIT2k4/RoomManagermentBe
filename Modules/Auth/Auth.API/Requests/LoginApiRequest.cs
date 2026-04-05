namespace Auth.API.Requests;

/// <summary>
/// Login payload from the client. IP and User-Agent are never taken from the client.
/// </summary>
public sealed record LoginApiRequest(string Login, string Password, bool RememberMe);
