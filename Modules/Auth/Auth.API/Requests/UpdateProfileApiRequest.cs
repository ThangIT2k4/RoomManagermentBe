namespace Auth.API.Requests;

public sealed record UpdateProfileApiRequest(string? FullName, DateOnly? Dob, short? Gender, string? Address, string? Note);
