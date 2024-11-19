using Microsoft.Extensions.DependencyInjection;
using YoutubeLinks.Api.Data.Database;

namespace YoutubeLinks.IntegrationTests;

public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>,
        IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly AppDbContext Context;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Client = factory.CreateClient();
    }

    public void Dispose()
    {
        _scope.Dispose();
        Context.Dispose();
        Client.Dispose();
    }
}