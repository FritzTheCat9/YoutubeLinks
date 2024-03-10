using Serilog;
using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Blazor.Logging
{
    public static class LoggingExtensions
    {
        private const string _sectionName = "Log";

        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<LogOptions>(configuration.GetRequiredSection(_sectionName));
            //var logOptions = configuration.GetOptions<LogOptions>(_sectionName);
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Information);

            return services;
        }
    }
}
