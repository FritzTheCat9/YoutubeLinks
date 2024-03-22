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
            app.MapPost("/api/links/download", async (
                DownloadLink.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var file = await mediator.Send(command, cancellationToken);
                return Results.File(file.FileBytes, file.ContentType, file.FileName);
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
                    var youtubeFile = command.YoutubeFileType switch
                    {
                        DownloadLink.YoutubeFileType.MP4 => await _youtubeService.GetMP4File(link.VideoId),
                        _ => await _youtubeService.GetMP3File(link.VideoId),
                    };

                    if (isUserPlaylist)
                    {
                        link.Downloaded = true;
                        await _linkRepository.Update(link);
                        await _linkRepository.SaveChanges();
                    }

                    return new DownloadLink.Response
                    {
                        FileBytes = youtubeFile.FileBytes,
                        ContentType = youtubeFile.ContentType,
                        FileName = youtubeFile.FileName,
                    };
                }
                else
                {
                    throw new MyForbiddenException();
                }
            }
        }
    }
}
