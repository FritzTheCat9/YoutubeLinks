using MediatR;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands
{
    public static class ImportPlaylistFromJsonFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/playlists/import", async (
                ImportPlaylistFromJson.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                int playlistId = await mediator.Send(command, cancellationToken);
                return Results.CreatedAtRoute("GetPlaylist", new { id = playlistId });
            })
                .WithTags("Playlists")
                .RequireAuthorization(Policy.User);

            return app;
        }

        public class Handler : IRequestHandler<ImportPlaylistFromJson.Command, int>
        {
            private readonly IPlaylistRepository _playlistRepository;
            private readonly IAuthService _authService;
            private readonly IClock _clock;

            public Handler(
                IPlaylistRepository playlistRepository,
                IAuthService authService,
                IClock clock)
            {
                _playlistRepository = playlistRepository;
                _authService = authService;
                _clock = clock;
            }

            public async Task<int> Handle(
                ImportPlaylistFromJson.Command command,
                CancellationToken cancellationToken)
            {
                var currentUserId = _authService.GetCurrentUserId() ?? throw new MyForbiddenException();

                var playlist = new Playlist
                {
                    Id = 0,
                    Created = _clock.Current(),
                    Modified = _clock.Current(),
                    Name = command.Name,
                    Public = command.Public,
                    UserId = currentUserId,
                };

                var links = new List<Link>();

                foreach (var exportedLink in command.ExportedLinks)
                {
                    var link = new Link
                    {
                        Id = 0,
                        Created = _clock.Current(),
                        Modified = _clock.Current(),
                        Url = exportedLink.Url,
                        VideoId = exportedLink.VideoId,
                        Title = exportedLink.Title,
                        Downloaded = false,
                    };

                    links.Add(link);
                }

                playlist.Links.AddRange(links);

                return await _playlistRepository.Create(playlist);
            }
        }
    }
}
