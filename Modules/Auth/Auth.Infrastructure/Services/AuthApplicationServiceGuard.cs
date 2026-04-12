using Microsoft.Extensions.Logging;
using RoomManagerment.Shared.Common;

namespace Auth.Infrastructure.Services;

/// <summary>
/// Catches unexpected exceptions from Auth application operations, logs them, and returns a uniform <see cref="Result"/> failure.
/// </summary>
internal static class AuthApplicationServiceGuard
{
    public static async Task<Result> RunAsync(
        ILogger logger,
        string operation,
        CancellationToken cancellationToken,
        Func<Task<Result>> action)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Auth application operation failed: {Operation}", operation);
            return Result.Failure(Error.Unexpected("Auth.Unexpected", "An unexpected error occurred while processing your request."));
        }
    }

    public static async Task<Result<T>> RunAsync<T>(
        ILogger logger,
        string operation,
        CancellationToken cancellationToken,
        Func<Task<Result<T>>> action)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Auth application operation failed: {Operation}", operation);
            return Result<T>.Failure(Error.Unexpected("Auth.Unexpected", "An unexpected error occurred while processing your request."));
        }
    }
}
