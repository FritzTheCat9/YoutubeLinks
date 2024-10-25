using Microsoft.EntityFrameworkCore;

namespace YoutubeLinks.Api.Data.Database;

public class DatabaseInitializer(
    IServiceProvider serviceProvider,
    ILogger<DatabaseInitializer> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError("[DatabaseInitializer] Error while creating and migrating the database: {0}", ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) 
        => Task.CompletedTask;
}