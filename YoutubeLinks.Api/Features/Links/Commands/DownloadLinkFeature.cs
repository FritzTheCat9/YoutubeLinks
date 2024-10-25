﻿using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Features.Links.Commands;

public static class DownloadLinkFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/links/download", async (
                DownloadLink.Command command,
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
    }

    public class Handler(
        IAuthService authService,
        ILinkRepository linkRepository,
        IYoutubeService youtubeService)
        : IRequestHandler<DownloadLink.Command, YoutubeFile>
    {
        public async Task<YoutubeFile> Handle(
            DownloadLink.Command command,
            CancellationToken cancellationToken)
        {
            var link = await linkRepository.Get(command.Id) ?? throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(link.Playlist.UserId);
            var isPublicPlaylist = link.Playlist.Public;

            if (!isUserPlaylist && !isPublicPlaylist)
            {
                throw new MyForbiddenException();
            }

            var downloader = YoutubeDownloaderHelpers.GetYoutubeDownloader(command.YoutubeFileType, youtubeService);
            var youtubeFile = await downloader.Download(link.VideoId, link.Title);

            return youtubeFile;
        }
    }
}