using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}