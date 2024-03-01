using YoutubeLinks.Api.Auth;
using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Api.Extensions
{
    public static class CORSExtensions
    {
        private const string _policyName = "MyCorsPolicy";
        private const string _sectionName = "Auth";

        public static IServiceCollection AddCORS(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AuthOptions>(configuration.GetRequiredSection(_sectionName));
            var authOptions = configuration.GetOptions<AuthOptions>(_sectionName);

            services.AddCors(options =>
            {
                options.AddPolicy(_policyName, policy =>
                {
                    policy.WithOrigins(authOptions.FrontendUrl)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }

        public static WebApplication UseCORS(this WebApplication app)
        {
            app.UseCors(_policyName);

            return app;
        }
    }
}
