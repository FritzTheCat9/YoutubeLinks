using MediatR;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands
{
    public static class UpdatePlaylistFeature
    {
        public static void Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/api/playlists/{id:int}", async (
                int id,
                UpdatePlaylist.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                command.Id = id;
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
                .WithTags(Tags.Playlists)
                .RequireAuthorization(Policy.User);
        }

        public class Handler : IRequestHandler<UpdatePlaylist.Command, Unit>
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

            public async Task<Unit> Handle(
                UpdatePlaylist.Command command,
                CancellationToken cancellationToken)
            {
                var playlist = await _playlistRepository.Get(command.Id) ?? throw new MyNotFoundException();
                
                var isUserPlaylist = _authService.IsLoggedInUser(playlist.UserId);
                if (!isUserPlaylist)
                    throw new MyForbiddenException();

                playlist.Name = command.Name;
                playlist.Public = command.Public;
                playlist.Modified = _clock.Current();

                await _playlistRepository.Update(playlist);
                return Unit.Value;
            }
        }
    }
}
