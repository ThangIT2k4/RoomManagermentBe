namespace Auth.API.Configuration;

/// <summary>
/// Maps flat <c>EMAIL_*</c> / <c>OTP_*</c> from process environment and from <c>.env</c> / <c>.env.local</c>
/// (walked upward from <see cref="Directory.GetCurrentDirectory"/>) into <c>Email:*</c> / <c>Otp:*</c>.
/// ASP.NET Core already binds <c>Email__Host</c>; this layer is for docker-style <c>EMAIL_HOST</c> without loading .env into the shell.
/// </summary>
public static class AuthEmailEnvConfiguration
{
    public static void Apply(ConfigurationManager configuration)
    {
        var fileVars = LoadEnvLikeFiles(Directory.GetCurrentDirectory());

        string? Resolve(string key)
        {
            var e = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(e))
            {
                return e.Trim();
            }

            return fileVars.TryGetValue(key, out var f) && !string.IsNullOrWhiteSpace(f) ? f.Trim() : null;
        }

        void Map(string legacyKey, string configKey)
        {
            var v = Resolve(legacyKey);
            if (!string.IsNullOrWhiteSpace(v))
            {
                configuration[configKey] = v;
            }
        }

        Map("EMAIL_HOST", "Email:Host");
        Map("EMAIL_PORT", "Email:Port");
        Map("EMAIL_USERNAME", "Email:Username");
        Map("EMAIL_PASSWORD", "Email:Password");
        Map("EMAIL_ENABLE_SSL", "Email:EnableSsl");
        Map("EMAIL_FROM_EMAIL", "Email:FromEmail");
        Map("EMAIL_FROM_NAME", "Email:FromName");
        Map("EMAIL_ALLOW_SEND_WITHOUT_SMTP", "Email:AllowSendWithoutSmtp");
        Map("OTP_RESEND_COOLDOWN_MINUTES", "Otp:ResendCooldownMinutes");
        Map("OTP_VERIFY_EMAIL_EXPIRY_MINUTES", "Otp:VerifyEmailExpiryMinutes");
        Map("OTP_RESET_PASSWORD_EXPIRY_MINUTES", "Otp:ResetPasswordExpiryMinutes");
    }

    private static Dictionary<string, string> LoadEnvLikeFiles(string startDirectory)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in EnumerateCandidateEnvFiles(startDirectory))
        {
            if (!File.Exists(path))
            {
                continue;
            }

            foreach (var line in File.ReadLines(path))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
                {
                    continue;
                }

                var separatorIndex = trimmed.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = trimmed[..separatorIndex].Trim();
                var value = trimmed[(separatorIndex + 1)..].Trim();

                if (value.Length >= 2 && value.StartsWith('"') && value.EndsWith('"'))
                {
                    value = value[1..^1];
                }

                result[key] = value;
            }
        }

        return result;
    }

    private static IEnumerable<string> EnumerateCandidateEnvFiles(string startDirectory)
    {
        var ordered = new List<string>();
        var current = new DirectoryInfo(startDirectory);
        while (current is not null)
        {
            ordered.Add(Path.Combine(current.FullName, ".env.local"));
            ordered.Add(Path.Combine(current.FullName, ".env"));
            current = current.Parent;
        }

        // Nearest directory to CWD wins over parents (later assignment overwrites).
        ordered.Reverse();
        return ordered;
    }
}
