using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;

namespace YoutubeLinks.Api.Features.Links.Commands
{
    public static class DownloadLinkFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/links/{id}/download", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DownloadLink.Command() { Id = id };
                var mp3File = await mediator.Send(command, cancellationToken);
                return Results.File(mp3File.FileBytes, "audio/mpeg", mp3File.FileName);
            })
                .WithTags("Links")
                .AllowAnonymous();

            return app;
        }

        public class Handler : IRequestHandler<DownloadLink.Command, DownloadLink.Response>
        {
            private readonly IAuthService _authService;
            private readonly ILinkRepository _linkRepository;
            private readonly IYoutubeService _youtubeService;

            public Handler(
                IAuthService authService,
                ILinkRepository linkRepository,
                IYoutubeService youtubeService)
            {
                _authService = authService;
                _linkRepository = linkRepository;
                _youtubeService = youtubeService;
            }

            public async Task<DownloadLink.Response> Handle(
                DownloadLink.Command command,
                CancellationToken cancellationToken)
            {
                var link = await _linkRepository.Get(command.Id) ?? throw new MyNotFoundException();

                var isUserPlaylist = _authService.IsLoggedInUser(link.Playlist.UserId);
                var isPublicPlaylist = link.Playlist.Public;

                if (isUserPlaylist || isPublicPlaylist)
                {
                    var (fileBytes, fileName) = await _youtubeService.GetMp3File(link.VideoId);

                    var response = new DownloadLink.Response
                    {
                        FileBytes = fileBytes,
                        FileName = fileName,
                    };

                    return response;
                }
                else
                {
                    throw new MyForbiddenException();
                }
            }
        }
    }
}
