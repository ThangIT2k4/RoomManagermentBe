namespace RoomManagerment.Shared.Common;

public sealed record ValidationFieldError(string Field, string Message);

public sealed record Error(string Code, string Message, ErrorType Type = ErrorType.BadRequest)
{
    /// <summary>Optional FluentValidation-style field errors (RFC 7807 extension <c>errors</c>).</summary>
    public IReadOnlyList<ValidationFieldError>? FieldErrors { get; init; }

    public static Error Validation(string code, string message, IReadOnlyList<ValidationFieldError>? fieldErrors = null) =>
        new(code, message, ErrorType.Validation) { FieldErrors = fieldErrors };

    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    public static Error BadRequest(string code, string message) =>
        new(code, message, ErrorType.BadRequest);

    public static Error Unexpected(string code, string message) =>
        new(code, message, ErrorType.Unexpected);
}

