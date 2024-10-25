using MediatR;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands
{
    public static class CreatePlaylistFeature
    {
        public static void Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/playlists", async (
                CreatePlaylist.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var playlistId = await mediator.Send(command, cancellationToken);
                return Results.CreatedAtRoute("GetPlaylist", new { id = playlistId });
            })
                .WithTags(Tags.Playlists)
                .RequireAuthorization(Policy.User);
        }

        public class Handler : IRequestHandler<CreatePlaylist.Command, int>
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
                CreatePlaylist.Command command,
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

                return await _playlistRepository.Create(playlist);
            }
        }
    }
}