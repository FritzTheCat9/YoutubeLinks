using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands;

public static class ResetLinksDownloadedFlagFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/playlists/resetDownloadedFlag", async (
                ResetLinksDownloadedFlag.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
            .WithTags(Tags.Playlists)
            .RequireAuthorization(Policy.User);
    }

    public class Handler(
        IPlaylistRepository playlistRepository,
        IAuthService authService)
        : IRequestHandler<ResetLinksDownloadedFlag.Command, Unit>
    {
        public async Task<Unit> Handle(
            ResetLinksDownloadedFlag.Command command,
            CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.Get(command.Id) ?? throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(playlist.UserId);
            if (!isUserPlaylist)
            {
                throw new MyForbiddenException();
            }

            await playlistRepository.SetLinksDownloadedFlag(playlist, command.IsDownloaded);

            return Unit.Value;
        }
    }
}