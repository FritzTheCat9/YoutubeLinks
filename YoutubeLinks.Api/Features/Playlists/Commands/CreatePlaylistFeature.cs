using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands;

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
                return Results.CreatedAtRoute("GetPlaylist", new { id = playlistId }, playlistId);
            })
            .WithTags(Tags.Playlists)
            .RequireAuthorization(Policy.User);
    }

    public class Handler(
        IUserRepository userRepository,
        IPlaylistRepository playlistRepository,
        IAuthService authService)
        : IRequestHandler<CreatePlaylist.Command, int>
    {
        public async Task<int> Handle(
            CreatePlaylist.Command command,
            CancellationToken cancellationToken)
        {
            var currentUserId = authService.GetCurrentUserId() ?? throw new MyForbiddenException();
            var user = await userRepository.Get(currentUserId) ?? throw new MyNotFoundException();

            var playlist = Playlist.Create(command.Name, command.Public, user);

            return await playlistRepository.Create(playlist);
        }
    }
}