namespace RoomManagerment.Shared.Common;

/// <summary>
/// Drives HTTP status mapping for API error responses. Avoid string heuristics on error codes.
/// </summary>
public enum ErrorType
{
    BadRequest = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5,
    Unexpected = 6
}
