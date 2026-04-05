namespace Auth.API.Requests;

public sealed record ChangePasswordApiRequest(string CurrentPassword, string NewPassword);
