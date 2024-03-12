using MediatR;
using System.Text.Json;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.Api.Features.Playlists.Commands
{

    public static class ExportPlaylistToJsonFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/playlists/export", async (
                ExportPlaylistToJson.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var jsonFile = await mediator.Send(command, cancellationToken);
                return Results.File(jsonFile.FileBytes, "application/json", jsonFile.FileName);
            })
                .WithTags("Playlists")
                .AllowAnonymous();

            return app;
        }

        public class Handler : IRequestHandler<ExportPlaylistToJson.Command, ExportPlaylistToJson.PlaylistJsonFile>
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

            public async Task<ExportPlaylistToJson.PlaylistJsonFile> Handle(
                ExportPlaylistToJson.Command command,
                CancellationToken cancellationToken)
            {
                var playlist = await _playlistRepository.Get(command.Id) ?? throw new MyNotFoundException();

                var isUserPlaylist = _authService.IsLoggedInUser(playlist.UserId);
                if (!playlist.Public
                    && !isUserPlaylist)
                    throw new MyForbiddenException();

                var links = playlist.Links.Select(x => new ExportPlaylistToJson.ExportedLinkModel()
                {
                    Title = x.Title,
                    Url = x.Url,
                    VideoId = x.VideoId
                }).OrderBy(x => x.Title);

                return new ExportPlaylistToJson.PlaylistJsonFile()
                {
                    FileBytes = JsonSerializer.SerializeToUtf8Bytes(links),
                    FileName = $"{playlist.Name}.json",
                };
            }
        }
    }
}
