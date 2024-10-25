using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands.ExportPlaylistFeature;

public static class ExportPlaylistFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
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
    }

    public class Handler(
        IPlaylistRepository playlistRepository,
        IAuthService authService)
        : IRequestHandler<ExportPlaylist.Command, PlaylistFile>
    {
        public async Task<PlaylistFile> Handle(
            ExportPlaylist.Command command,
            CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.Get(command.Id) ?? throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(playlist.UserId);
            if (!playlist.Public
                && !isUserPlaylist)
                throw new MyForbiddenException();

            var exporter = PlaylistExporterHelpers.GetExporter(command.PlaylistFileType);

            var playlistFile = exporter.Export(playlist);

            return playlistFile;
        }
    }
}