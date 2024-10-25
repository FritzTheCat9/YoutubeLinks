using MediatR;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands;

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

    public class Handler(
        IPlaylistRepository playlistRepository,
        IAuthService authService,
        IClock clock)
        : IRequestHandler<UpdatePlaylist.Command, Unit>
    {
        public async Task<Unit> Handle(
            UpdatePlaylist.Command command,
            CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.Get(command.Id) ?? throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(playlist.UserId);
            if (!isUserPlaylist)
            {
                throw new MyForbiddenException();
            }

            playlist.Name = command.Name;
            playlist.Public = command.Public;
            playlist.Modified = clock.Current();

            await playlistRepository.Update(playlist);
            return Unit.Value;
        }
    }
}