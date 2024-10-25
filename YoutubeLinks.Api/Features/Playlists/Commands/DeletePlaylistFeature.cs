using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands;

public static class DeletePlaylistFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/playlists/{id:int}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeletePlaylist.Command { Id = id };
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
            .WithTags(Tags.Playlists)
            .RequireAuthorization(Policy.User);
    }

    public class Handler(
        IPlaylistRepository playlistRepository,
        IAuthService authService)
        : IRequestHandler<DeletePlaylist.Command, Unit>
    {
        public async Task<Unit> Handle(
            DeletePlaylist.Command command,
            CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.Get(command.Id) ?? throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(playlist.UserId);
            if (!isUserPlaylist)
            {
                throw new MyForbiddenException();
            }

            await playlistRepository.Delete(playlist);
            return Unit.Value;
        }
    }
}