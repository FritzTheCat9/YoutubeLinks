using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands.ImportPlaylistFeature;

public static class ImportPlaylistFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/playlists/import", async (
                ImportPlaylist.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var playlistId = await mediator.Send(command, cancellationToken);
                return Results.CreatedAtRoute("GetPlaylist", new { id = playlistId });
            })
            .WithTags(Tags.Playlists)
            .RequireAuthorization(Policy.User);
    }

    public class Handler(
        IPlaylistRepository playlistRepository,
        IUserRepository userRepository,
        IAuthService authService,
        IYoutubeService youtubeService,
        IStringLocalizer<ApiValidationMessage> localizer)
        : IRequestHandler<ImportPlaylist.Command, int>
    {
        public async Task<int> Handle(
            ImportPlaylist.Command command,
            CancellationToken cancellationToken)
        {
            var currentUserId = authService.GetCurrentUserId() ?? throw new MyForbiddenException();
            var user = await userRepository.Get(currentUserId) ?? throw new MyForbiddenException();
            
            var playlist = Playlist.Create(command.Name, command.Public, user);

            var importer = PlaylistImporterHelpers.GetImporter(command.PlaylistFileType);

            await importer.Import(youtubeService, localizer, command, playlist);

            return await playlistRepository.Create(playlist);
        }
    }
}