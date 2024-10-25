using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationContext = new ValidationContext<TRequest>(request);

        var errors = validators
            .Select(validator => validator.Validate(validationContext))
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage))
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage)
                .ToList());

        if (errors.Count != 0)
        {
            throw new MyValidationException(errors);
        }

        var result = await next();
        return result;
    }

    private class ValidationError(string propertyName, string errorMessage)
    {
        public string PropertyName { get; } = propertyName;
        public string ErrorMessage { get; } = errorMessage;
    }
}