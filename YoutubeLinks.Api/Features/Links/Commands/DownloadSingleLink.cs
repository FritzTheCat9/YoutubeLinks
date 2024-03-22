using MediatR;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Features.Links.Commands
{
    public static class DownloadSingleLinkFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/links/downloadSingle", async (
                DownloadSingleLink.Command command,
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

        public class Handler : IRequestHandler<DownloadSingleLink.Command, DownloadSingleLink.Response>
        {
            private readonly IYoutubeService _youtubeService;

            public Handler(
                IYoutubeService youtubeService)
            {
                _youtubeService = youtubeService;
            }

            public async Task<DownloadSingleLink.Response> Handle(
                DownloadSingleLink.Command command,
                CancellationToken cancellationToken)
            {
                var videoId = YoutubeHelpers.GetVideoId(command.Url);

                var youtubeFile = command.YoutubeFileType switch
                {
                    DownloadSingleLink.YoutubeFileType.MP4 => await _youtubeService.GetMP4File(videoId),
                    _ => await _youtubeService.GetMP3File(videoId),
                };

                return new DownloadSingleLink.Response
                {
                    FileBytes = youtubeFile.FileBytes,
                    ContentType = youtubeFile.ContentType,
                    FileName = youtubeFile.FileName,
                };
            }
        }
    }
}