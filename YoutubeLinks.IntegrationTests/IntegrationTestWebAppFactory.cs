using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using Testcontainers.MsSql;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Sdk.Clients;

namespace YoutubeLinks.IntegrationTests;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:latest")
        .WithPassword("Password1!")
        .Build();

    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("test");

        builder.ConfigureTestServices(services =>
        {
            AddTestDatabase(services);
            AddTestApiClient(services);
        });
    }

    private void AddTestApiClient(IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, TestJwtProvider>();

        // Replace real YoutubeService with a test implementation so integration tests
        // don't call external binaries (yt-dlp / ffmpeg) during runs.
        services.RemoveAll<YoutubeLinks.Api.Services.IYoutubeService>();
        services.AddScoped<YoutubeLinks.Api.Services.IYoutubeService, TestYoutubeService>();
            
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.AddApiClients(configuration);
            
        services.RemoveAll<HttpClient>();
        services.AddScoped(sp => CreateClient());
    }

    private void AddTestDatabase(IServiceCollection services)
    {
        services.RemoveAll<DbContextOptions<AppDbContext>>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(_dbContainer.GetConnectionString())
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        
        // using var scope = services.BuildServiceProvider().CreateScope();
        // var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // context.Database.Migrate();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Ensure database schema is created/migrated before tests run.
        try
        {
            var options = new DbContextOptionsBuilder<YoutubeLinks.Api.Data.Database.AppDbContext>()
                .UseSqlServer(_dbContainer.GetConnectionString())
                .Options;

            using var ctx = new YoutubeLinks.Api.Data.Database.AppDbContext(options);
            // Prefer migrations when available; fallback to EnsureCreated
            try
            {
                ctx.Database.Migrate();
            }
            catch
            {
                ctx.Database.EnsureCreated();
            }

            // Seed expected integration test users (if not present)
            try
            {
                var passwordService = new PasswordService(new Microsoft.AspNetCore.Identity.PasswordHasher<User>());

                if (!ctx.Users.Any(u => u.Email.Value == "ytlinksapp@gmail.com"))
                {
                    var admin = User.Create("ytlinksapp@gmail.com", "ytlinksapp", YoutubeLinks.Shared.Features.Users.Helpers.ThemeColor.Light, true, true);
                    admin.SetPassword("Asd123!", passwordService);
                    ctx.Users.Add(admin);
                }

                if (!ctx.Users.Any(u => u.Email.Value == "ytlinksapp1@gmail.com"))
                {
                    var user = User.Create("ytlinksapp1@gmail.com", "ytlinksapp1", YoutubeLinks.Shared.Features.Users.Helpers.ThemeColor.Light, false, true);
                    user.SetPassword("Asd123!", passwordService);
                    ctx.Users.Add(user);
                }

                await ctx.SaveChangesAsync();
            }
            catch
            {
                // ignore seed errors and let tests surface issues
            }
        }
        catch
        {
            // If migration/setup fails here, tests will show schema errors. Let failures surface in test run.
        }
    }

    public async new Task DisposeAsync()
    {
        // Ensure WebApplicationFactory cleans up its resources
        try
        {
            await base.DisposeAsync();
        }
        catch
        {
            // ignore
        }

        await _dbContainer.StopAsync();
    }
}