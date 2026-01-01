using NSubstitute;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.UnitTests.Features.Links.Commands;

public class DownloadSingleLinkFeatureTests
{
    private readonly IYoutubeService _youtubeService = Substitute.For<IYoutubeService>();

    [Fact]
    public async Task DownloadSingleLinkHandler_ReturnsYoutubeFile()
    {
        var command = new DownloadSingleLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            YoutubeFileType = YoutubeFileType.Mp3
        };
        var youtubeFile = new YoutubeFile();

        _youtubeService.GetMp3File(Arg.Any<string>()).Returns(youtubeFile);

        var handler = new DownloadSingleLinkFeature.Handler(_youtubeService);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(youtubeFile, result);
        await _youtubeService.Received().GetMp3File(Arg.Any<string>());
        await _youtubeService.DidNotReceive().GetMp4File(Arg.Any<string>());
    }
}
