using YoutubeLinks.Api.Auth;
using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Api.Extensions
{
    public static class MyCorsExtensions
    {
        private const string PolicyName = "MyCorsPolicy";
        private const string SectionName = "Auth";

        public static IServiceCollection AddMyCors(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AuthOptions>(configuration.GetRequiredSection(SectionName));
            var authOptions = configuration.GetOptions<AuthOptions>(SectionName);

            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins(authOptions.FrontendUrl)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithExposedHeaders("Content-Disposition");
                });
            });

            return services;
        }

        public static WebApplication UseMyCors(this WebApplication app)
        {
            app.UseCors(PolicyName);

            return app;
        }
    }
}
