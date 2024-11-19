using FluentAssertions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Links.Commands;

public class DownloadSingleLinkFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DownloadSingleLink_ShouldSucceed_WhenDataIsValid()
    {
        var command = new DownloadSingleLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            YoutubeFileType = YoutubeFileType.Mp3,
        };

        const string title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
        
        var response = await LinkApiClient.DownloadSingleLink(command);
        response.IsSuccessStatusCode.Should().BeTrue();

        await using (var stream = await response.Content.ReadAsStreamAsync())
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Length.Should().BeGreaterThan(0);
        }

        var filename = response.Content.Headers.ContentDisposition?.FileNameStar;
        var linkTitleWithExtension = $"{title}.{YoutubeHelpers.YoutubeFileTypeToString(YoutubeFileType.Mp3)}";
        filename.Should().Be(linkTitleWithExtension);
    }
}