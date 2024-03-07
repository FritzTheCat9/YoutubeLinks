using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Blazor.Clients
{
    public static class ApiClientsExtensions
    {
        private const string _sectionName = "Api";

        public static IServiceCollection AddApiClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiOptions>(configuration.GetRequiredSection(_sectionName));
            var apiOptions = configuration.GetOptions<ApiOptions>(_sectionName);

            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiOptions.Url) });

            services.AddScoped<IApiClient, ApiClient>();
            services.AddScoped<IUserApiClient, UserApiClient>();
            services.AddScoped<IPlaylistApiClient, PlaylistApiClient>();
            services.AddScoped<ILinkApiClient, LinkApiClient>();

            return services;
        }
    }
}
