using MediatR;
using System.Diagnostics;

namespace YoutubeLinks.Api.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).FullName?.Split('.').LastOrDefault();

            _logger.LogInformation("[MediatR] Starting request {RequestName}", requestName);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation("[MediatR] Completed request {RequestName} in {Elapsed}", requestName, stopwatch.Elapsed);

            return response;
        }
    }
}
