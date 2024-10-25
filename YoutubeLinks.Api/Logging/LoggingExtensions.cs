using Serilog;
using Serilog.Events;
using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Api.Logging;

public static class LoggingExtensions
{
    private const string SectionName = "Log";

    private const string LogTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3} {CorrelationId}] {Message}{NewLine}{Exception}";

    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LogOptions>(configuration.GetRequiredSection(SectionName));
        var logOptions = configuration.GetOptions<LogOptions>(SectionName);

        services.AddSerilog(x =>
        {
            x.Enrich.WithCorrelationIdHeader();
            x.WriteTo.Console(outputTemplate: LogTemplate);
            x.WriteTo.File(logOptions.FilePath, rollingInterval: RollingInterval.Day, outputTemplate: LogTemplate);
            x.WriteTo.Seq(logOptions.SeqUrl);
            x.MinimumLevel.Information();
            x.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information);
            x.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning);
        });

        return services;
    }
}