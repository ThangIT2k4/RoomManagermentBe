using Microsoft.AspNetCore.Http;

namespace  Identity.Infrastructure.Options;

public class SessionOptions
{
    public int IdleTimeout { get; set; } = 30;
    public CookieOptions Cookie { get; set; } = new();
}

public class CookieOptions
{
    public bool HttpOnly { get; set; } = true;
    public bool IsEssential { get; set; } = true;
    public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
    public CookieSecurePolicy SecurePolicy { get; set; } = CookieSecurePolicy.Always;
}

