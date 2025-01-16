using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Helpers;

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
            logger.LogInformation("[DatabaseInitializer] Applying migrations...");
            await dbContext.Database.MigrateAsync(cancellationToken);

            logger.LogInformation("[DatabaseInitializer] Checking for existing users...");
            if (!dbContext.Users.Any())
            {
                var admin = User.Create("ytlinksapp@gmail.com", "Admin", ThemeColor.Light, true, true);
                admin.SetPasswordHash("AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==");

                var user = User.Create("ytlinksapp1@gmail.com", "User", ThemeColor.Light, false, true);
                user.SetPasswordHash("AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==");

                dbContext.Users.AddRange(admin, user);
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation("[DatabaseInitializer] Default users added successfully.");
            }
            else
            {
                logger.LogInformation("[DatabaseInitializer] Users already exist. Skipping seeding.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError("[DatabaseInitializer] Error while creating and migrating the database: {0}", ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}