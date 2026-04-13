namespace Auth.API.Requests;

public sealed record ResendVerifyEmailOtpApiRequest(string Email, string Password);
