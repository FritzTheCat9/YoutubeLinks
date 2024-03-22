using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands
{
    public static class ResetLinksDownloadedFlagFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/playlists/resetDownloadedFlag", async (
                ResetLinksDownloadedFlag.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
                .WithTags("Playlists")
                .RequireAuthorization(Policy.User);

            return app;
        }

        public class Handler : IRequestHandler<ResetLinksDownloadedFlag.Command, Unit>
        {
            private readonly IPlaylistRepository _playlistRepository;
            private readonly IAuthService _authService;

            public Handler(
                IPlaylistRepository playlistRepository,
                IAuthService authService)
            {
                _playlistRepository = playlistRepository;
                _authService = authService;
            }

            public async Task<Unit> Handle(
                ResetLinksDownloadedFlag.Command command,
                CancellationToken cancellationToken)
            {
                var playlist = await _playlistRepository.Get(command.Id) ?? throw new MyNotFoundException();

                var isUserPlaylist = _authService.IsLoggedInUser(playlist.UserId);
                if (!isUserPlaylist)
                    throw new MyForbiddenException();

                await _playlistRepository.SetLinksDownloadedFlag(playlist, command.IsDownloaded);

                return Unit.Value;
            }
        }
    }
}