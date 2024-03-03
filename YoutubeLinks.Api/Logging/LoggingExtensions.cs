using Serilog;
using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Api.Logging
{
    public static class LoggingExtensions
    {
        private const string _sectionName = "Log";
        private const string _logTemplate = "[{Timestamp:HH:mm:ss} {Level:u3} {CorrelationId}] {Message}{NewLine}{Exception}";

        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LogOptions>(configuration.GetRequiredSection(_sectionName));
            var logOptions = configuration.GetOptions<LogOptions>(_sectionName);

            services.AddSerilog(x =>
            {
                x.Enrich.WithCorrelationIdHeader();
                x.WriteTo.Console(outputTemplate: _logTemplate);
                x.WriteTo.File(logOptions.FilePath, rollingInterval: RollingInterval.Day, outputTemplate: _logTemplate);
                x.MinimumLevel.Information();
                x.MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Information);
                x.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning);
            });

            return services;
        }
    }
}
