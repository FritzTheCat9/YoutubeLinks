﻿using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands.ExportPlaylistFeature
{

    public static class ExportPlaylistFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/playlists/export", async (
                ExportPlaylist.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var playlistFile = await mediator.Send(command, cancellationToken);
                return Results.File(playlistFile.FileBytes, playlistFile.ContentType, playlistFile.FileName);
            })
                .WithTags(Tags.Playlists)
                .AllowAnonymous();

            return app;
        }

        public class Handler : IRequestHandler<ExportPlaylist.Command, PlaylistFile>
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

            public async Task<PlaylistFile> Handle(
                ExportPlaylist.Command command,
                CancellationToken cancellationToken)
            {
                var playlist = await _playlistRepository.Get(command.Id) ?? throw new MyNotFoundException();

                var isUserPlaylist = _authService.IsLoggedInUser(playlist.UserId);
                if (!playlist.Public
                    && !isUserPlaylist)
                    throw new MyForbiddenException();

                var exporter = PlaylistExporterHelpers.GetExporter(command.PlaylistFileType);

                var playlistFile = exporter.Export(playlist);

                return playlistFile;
            }
        }
    }
}
