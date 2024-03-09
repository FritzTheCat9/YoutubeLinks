namespace YoutubeLinks.Api.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(
            this IServiceCollection services)
        {
            services.AddScoped<IYoutubeService, YoutubeService>();

            return services;
        }
    }
}
