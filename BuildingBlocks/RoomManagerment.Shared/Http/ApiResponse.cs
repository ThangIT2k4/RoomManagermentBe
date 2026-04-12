namespace RoomManagerment.Shared.Http;

public sealed class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public static ApiResponse<T> Succeed(T data, string message = "") =>
        new()
        {
            Success = true,
            Message = message ?? string.Empty,
            Data = data
        };

    public static ApiResponse<T> Failure(string message) =>
        new()
        {
            Success = false,
            Message = message,
            Data = default
        };
}
