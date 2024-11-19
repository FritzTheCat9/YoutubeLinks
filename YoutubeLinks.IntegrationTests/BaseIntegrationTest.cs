using Microsoft.Extensions.DependencyInjection;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Shared.Clients;

namespace YoutubeLinks.IntegrationTests;

public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>,
        IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly AppDbContext Context;
    protected readonly HttpClient Client;
    protected readonly IApiClient ApiClient;
    protected readonly IUserApiClient UserApiClient;
    protected readonly IPlaylistApiClient PlaylistApiClient;
    protected readonly ILinkApiClient LinkApiClient;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Client = factory.CreateClient();
        ApiClient = _scope.ServiceProvider.GetRequiredService<IApiClient>();
        UserApiClient = _scope.ServiceProvider.GetRequiredService<IUserApiClient>();
        PlaylistApiClient = _scope.ServiceProvider.GetRequiredService<IPlaylistApiClient>();
        LinkApiClient = _scope.ServiceProvider.GetRequiredService<ILinkApiClient>();
    }

    public void Dispose()
    {
        _scope.Dispose();
        Context.Dispose();
        Client.Dispose();
    }
}