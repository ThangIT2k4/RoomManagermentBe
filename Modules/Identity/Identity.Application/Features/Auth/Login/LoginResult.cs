namespace Identity.Application.Features.Auth.Login;

public class LoginResult
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string AccessToken { get; set; }
    public DateTime Expiration { get; set; }
}