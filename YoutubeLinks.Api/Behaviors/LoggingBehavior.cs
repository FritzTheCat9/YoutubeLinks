using System.Diagnostics;
using MediatR;

namespace YoutubeLinks.Api.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).FullName?.Split('.').LastOrDefault();

        logger.LogInformation("[MediatR] Starting request {RequestName}", requestName);

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var response = await next();

        stopwatch.Stop();

        logger.LogInformation("[MediatR] Completed request {RequestName} in {Elapsed}", requestName,
            stopwatch.Elapsed);

        return response;
    }
}