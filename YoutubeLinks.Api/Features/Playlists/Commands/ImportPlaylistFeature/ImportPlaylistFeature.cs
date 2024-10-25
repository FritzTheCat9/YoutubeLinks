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

    public class Handler : IRequestHandler<ImportPlaylist.Command, int>
    {
        private readonly IAuthService _authService;
        private readonly IClock _clock;
        private readonly IStringLocalizer<ApiValidationMessage> _localizer;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IYoutubeService _youtubeService;

        public Handler(
            IPlaylistRepository playlistRepository,
            IAuthService authService,
            IYoutubeService youtubeService,
            IClock clock,
            IStringLocalizer<ApiValidationMessage> localizer)
        {
            _playlistRepository = playlistRepository;
            _authService = authService;
            _youtubeService = youtubeService;
            _clock = clock;
            _localizer = localizer;
        }

        public async Task<int> Handle(
            ImportPlaylist.Command command,
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
                UserId = currentUserId
            };

            var importer = PlaylistImporterHelpers.GetImporter(command.PlaylistFileType);

            var links = (await importer.Import(_youtubeService, _clock, _localizer, command)).ToList();

            playlist.Links.AddRange(links);

            return await _playlistRepository.Create(playlist);
        }
    }
}