using YoutubeLinks.Api.Features.Users.Extensions;

namespace YoutubeLinks.Api.Extensions
{
    public static class EndpointsExtensions
    {
        public static WebApplication AddEndpoints(this WebApplication app)
        {
            app.AddUserEndpoints();

            return app;
        }
    }
}
