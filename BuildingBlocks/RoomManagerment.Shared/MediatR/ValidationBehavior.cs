using FluentValidation;
using MediatR;
using RoomManagerment.Shared.Common;

namespace RoomManagerment.Shared.MediatR;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!ResultPipeline.IsResultResponse(typeof(TResponse)))
        {
            return await next();
        }

        var validatorList = validators as IValidator<TRequest>[] ?? validators.ToArray();
        if (validatorList.Length == 0)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<FluentValidation.Results.ValidationFailure>();
        foreach (var validator in validatorList)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors.Where(f => f is not null));
        }

        if (failures.Count == 0)
        {
            return await next();
        }

        var fieldErrors = failures
            .Select(f => new ValidationFieldError(f.PropertyName, f.ErrorMessage))
            .ToList();

        var summary = string.Join("; ", failures.Select(f => f.ErrorMessage).Distinct());
        var error = Error.Validation("Validation.Failed", string.IsNullOrWhiteSpace(summary) ? "Validation failed." : summary, fieldErrors);

        return (TResponse)ResultPipeline.FailureInstance(typeof(TResponse), error);
    }
}
