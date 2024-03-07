using YoutubeLinks.Api.Features.Links.Extensions;
using YoutubeLinks.Api.Features.Playlists.Extensions;
using YoutubeLinks.Api.Features.Users.Extensions;

namespace YoutubeLinks.Api.Extensions
{
    public static class EndpointsExtensions
    {
        public static WebApplication AddEndpoints(this WebApplication app)
        {
            app.AddUserEndpoints();
            app.AddPlaylistsEndpoints();
            app.AddLinksEndpoints();

            return app;
        }
    }
}
