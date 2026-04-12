using System.Collections;
using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoomManagerment.Shared.Common;
using RoomManagerment.Shared.Messaging;

namespace CRM.Infrastructure.Services;

public sealed class AppRequestSender(IServiceProvider serviceProvider, ILogger<AppRequestSender> logger)
    : IAppSender
{
    public async Task<TResponse> Send<TResponse>(IAppRequest<TResponse> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var requestType = request.GetType();
        var requestName = requestType.Name;
        logger.LogInformation("Handling {RequestName}", requestName);
        var sw = Stopwatch.StartNew();

        try
        {
            if (typeof(TResponse) == typeof(Result) || (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>)))
            {
                var validationOutcome = await RunValidatorsAsync(request, requestType, cancellationToken);
                if (validationOutcome is { Count: > 0 })
                {
                    sw.Stop();
                    logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms (validation failed)", requestName, sw.ElapsedMilliseconds);
                    return CreateValidationFailure<TResponse>(validationOutcome);
                }
            }

            var handler = ResolveHandler(requestType, typeof(TResponse));
            var handleMethod = handler.GetType().GetMethod("Handle")
                ?? throw new InvalidOperationException($"Handler {handler.GetType().Name} has no Handle method.");
            var task = (Task)handleMethod.Invoke(handler, [request, cancellationToken])!;
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result")
                ?? throw new InvalidOperationException("Handler did not return a Task with Result.");
            var response = resultProperty.GetValue(task);
            if (response is null)
            {
                throw new InvalidOperationException("Handler returned null.");
            }

            sw.Stop();
            logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
            return (TResponse)response;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unhandled exception in handler for {RequestName}", requestName);
            throw;
        }
    }

    private object ResolveHandler(Type requestType, Type responseType)
    {
        var handlerInterface = typeof(IAppRequestHandler<,>).MakeGenericType(requestType, responseType);
        return serviceProvider.GetRequiredService(handlerInterface);
    }

    private async Task<List<ValidationFailure>?> RunValidatorsAsync(
        object request,
        Type requestType,
        CancellationToken cancellationToken)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(validatorType);
        var validatorsObj = serviceProvider.GetService(enumerableType);
        if (validatorsObj is not IEnumerable enumerable)
        {
            return null;
        }

        var failures = new List<ValidationFailure>();
        foreach (var validator in enumerable)
        {
            if (validator is null) continue;
            dynamic v = validator;
            dynamic req = request;
            ValidationResult result = await v.ValidateAsync(req, cancellationToken);
            failures.AddRange(result.Errors.Where(f => f is not null));
        }

        return failures.Count == 0 ? null : failures;
    }

    private static TResponse CreateValidationFailure<TResponse>(List<ValidationFailure> failures)
    {
        var fieldErrors = failures
            .Select(f => new ValidationFieldError(f.PropertyName, f.ErrorMessage))
            .ToList();
        var summary = string.Join("; ", failures.Select(f => f.ErrorMessage).Distinct());
        var error = Error.Validation("Validation.Failed", string.IsNullOrWhiteSpace(summary) ? "Validation failed." : summary, fieldErrors);

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = typeof(TResponse).GetMethod("Failure", [typeof(Error)])
                ?? throw new InvalidOperationException($"{typeof(TResponse).Name} has no static Failure(Error).");
            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException($"{typeof(TResponse).Name} is not a Result type; validation cannot be synthesized.");
    }
}
