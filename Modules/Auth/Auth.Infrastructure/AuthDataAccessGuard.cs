using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure;

/// <summary>
/// Wraps Auth persistence calls: logs infrastructure failures with repository context, then rethrows (no silent swallow).
/// </summary>
internal static class AuthDataAccessGuard
{
    public static async Task RunAsync(
        ILogger logger,
        string repositoryName,
        string operation,
        CancellationToken cancellationToken,
        Func<Task> action)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Truy cập dữ liệu Auth thất bại. Repository={Repository} Operation={Operation}", repositoryName, operation);
            throw;
        }
    }

    public static async Task<T> RunAsync<T>(
        ILogger logger,
        string repositoryName,
        string operation,
        CancellationToken cancellationToken,
        Func<Task<T>> action)
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
            logger.LogError(ex, "Truy cập dữ liệu Auth thất bại. Repository={Repository} Operation={Operation}", repositoryName, operation);
            throw;
        }
    }
}
