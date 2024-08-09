﻿using MediatR;
using YoutubeLinks.Api.Helpers;
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

                var fileStream = new FileStream(file.FilePath,
                                                FileMode.Open,
                                                FileAccess.Read,
                                                FileShare.Read,
                                                4096,
                                                FileOptions.DeleteOnClose);

                return Results.Stream(fileStream, file.ContentType, file.FileName, enableRangeProcessing: true);
            })
                .WithTags(Tags.Links)
                .AllowAnonymous();

            return app;
        }

        public class Handler : IRequestHandler<DownloadSingleLink.Command, YoutubeFile>
        {
            private readonly IYoutubeService _youtubeService;

            public Handler(
                IYoutubeService youtubeService)
            {
                _youtubeService = youtubeService;
            }

            public async Task<YoutubeFile> Handle(
                DownloadSingleLink.Command command,
                CancellationToken cancellationToken)
            {
                var videoId = YoutubeHelpers.GetVideoId(command.Url);

                var downloader = YoutubeDownloaderHelpers.GetYoutubeDownloader(command.YoutubeFileType, _youtubeService);
                var youtubeFile = await downloader.Download(videoId);

                return youtubeFile;
            }
        }
    }
}