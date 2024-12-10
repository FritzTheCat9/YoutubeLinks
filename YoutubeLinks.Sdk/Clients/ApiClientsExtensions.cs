using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Sdk.Clients;

public static class ApiClientsExtensions
{
    private const string SectionName = "Api";

    public static IServiceCollection AddApiClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ApiOptions>(configuration.GetRequiredSection(SectionName));
        var apiOptions = configuration.GetOptions<ApiOptions>(SectionName);

        services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(apiOptions.Url),
            Timeout = TimeSpan.FromMinutes(60)
        });

        services.AddScoped<IApiClient, ApiClient>();
        services.AddScoped<IUserApiClient, UserApiClient>();
        services.AddScoped<IPlaylistApiClient, PlaylistApiClient>();
        services.AddScoped<ILinkApiClient, LinkApiClient>();

        return services;
    }
}