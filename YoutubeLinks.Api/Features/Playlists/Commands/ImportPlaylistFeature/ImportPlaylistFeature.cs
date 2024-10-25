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
        IAuthService authService,
        IYoutubeService youtubeService,
        IClock clock,
        IStringLocalizer<ApiValidationMessage> localizer)
        : IRequestHandler<ImportPlaylist.Command, int>
    {
        public async Task<int> Handle(
            ImportPlaylist.Command command,
            CancellationToken cancellationToken)
        {
            var currentUserId = authService.GetCurrentUserId() ?? throw new MyForbiddenException();

            var playlist = new Playlist
            {
                Id = 0,
                Created = clock.Current(),
                Modified = clock.Current(),
                Name = command.Name,
                Public = command.Public,
                UserId = currentUserId
            };

            var importer = PlaylistImporterHelpers.GetImporter(command.PlaylistFileType);

            var links = (await importer.Import(youtubeService, clock, localizer, command)).ToList();

            playlist.Links.AddRange(links);

            return await playlistRepository.Create(playlist);
        }
    }
}