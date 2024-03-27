﻿using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.UnitTests.Features.Links.Commands
{
    public class DownloadSingleLinkFeatureTests
    {
        [Fact]
        public async Task DownloadSingleLinkHandler_ReturnsYoutubeFile()
        {
            var command = new DownloadSingleLink.Command
            {
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley",
                YoutubeFileType = YoutubeFileType.MP3,
            };
            var youtubeFile = new YoutubeFile();

            var youtubeService = Substitute.For<IYoutubeService>();
            var mediator = Substitute.For<IMediator>();

            youtubeService.GetMP3File(Arg.Any<string>()).Returns(youtubeFile);

            mediator.Send(Arg.Any<DownloadSingleLink.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DownloadSingleLinkFeature.Handler(youtubeService);
                    return handler.Handle(callInfo.Arg<DownloadSingleLink.Command>(), CancellationToken.None);
                });

            var result = await mediator.Send(command, CancellationToken.None);

            result.Should().Be(youtubeFile);
            await youtubeService.Received().GetMP3File(Arg.Any<string>());
            await youtubeService.DidNotReceive().GetMP4File(Arg.Any<string>());
        }
    }
}
