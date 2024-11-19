using FluentAssertions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Links.Commands;

public class DownloadLinkFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DownloadLink_ShouldSucceed_WhenDataIsValid()
    {
        var user = await LoginAsAdmin();

        var link = new Link
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            VideoId = "dQw4w9WgXcQ",
            Title = "Rick Astley - Never Gonna Give You Up",
            Downloaded = false,
        };

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        playlist.Links.Add(link);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new DownloadLink.Command
        {
            Id = link.Id,
            YoutubeFileType = YoutubeFileType.Mp3,
        };

        var response = await LinkApiClient.DownloadLink(command);
        response.IsSuccessStatusCode.Should().BeTrue();

        await using (var stream = await response.Content.ReadAsStreamAsync())
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Length.Should().BeGreaterThan(0);
        }

        var filename = response.Content.Headers.ContentDisposition?.FileNameStar;
        var linkTitleWithExtension = $"{link.Title}.{YoutubeHelpers.YoutubeFileTypeToString(YoutubeFileType.Mp3)}";
        filename.Should().Be(linkTitleWithExtension);
    }
}