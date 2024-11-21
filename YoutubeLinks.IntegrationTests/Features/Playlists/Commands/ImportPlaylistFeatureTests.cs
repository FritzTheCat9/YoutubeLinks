using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Commands;

public class ImportPlaylistFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ImportPlaylistFromTxtFile_ShouldSucceed_WhenDataIsValid()
    {
        var user = await LoginAsAdmin();

        var command = new ImportPlaylist.Command
        {
            Name = "TestPlaylist Txt",
            Public = true,
            ExportedLinkUrls =
            [
                "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                "https://www.youtube.com/watch?v=GtUVQei3nX4",
                "https://www.youtube.com/watch?v=u15tEo0wsQI",
            ],
            PlaylistFileType = PlaylistFileType.Txt,
        };

        await PlaylistApiClient.ImportPlaylist(command);

        var importedPlaylist = await Context.Playlists
            .Include(playlist => playlist.Links)
            .FirstOrDefaultAsync(x => x.Name == command.Name);

        importedPlaylist.Should().NotBeNull();
        importedPlaylist?.Name.Should().Be(command.Name);
        importedPlaylist?.Public.Should().Be(command.Public);
        importedPlaylist?.Links.Should().HaveCount(command.ExportedLinkUrls.Count);
        importedPlaylist?.UserId.Should().Be(user.UserId);

        foreach (var url in command.ExportedLinkUrls)
        {
            var matchingUrl = importedPlaylist?.Links.FirstOrDefault(x => x.Url == url);

            matchingUrl.Should().NotBeNull();
            matchingUrl.Should().Match<Link>(x => x.Url == url);
        }
    }

    [Fact]
    public async Task ImportPlaylistFromJsonFile_ShouldSucceed_WhenDataIsValid()
    {
        var user = await LoginAsAdmin();

        var command = new ImportPlaylist.Command
        {
            Name = "TestPlaylist Json",
            Public = true,
            ExportedLinks =
            [
                new LinkJsonModel
                {
                    Title = "Rick Astley - Never Gonna Give You Up (Official Music Video)",
                    Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                    VideoId = "dQw4w9WgXcQ",
                },
                new LinkJsonModel
                {
                    Title = "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams",
                    Url = "https://www.youtube.com/watch?v=GtUVQei3nX4",
                    VideoId = "GtUVQei3nX4",
                },
                new LinkJsonModel
                {
                    Title = "Dawid Podsiadło, P.T. Adamczyk — Phantom Liberty (Official Cyberpunk 2077 Music Video)",
                    Url = "https://www.youtube.com/watch?v=u15tEo0wsQI",
                    VideoId = "u15tEo0wsQI",
                },
            ],
            PlaylistFileType = PlaylistFileType.Json,
        };

        await PlaylistApiClient.ImportPlaylist(command);

        var importedPlaylist = await Context.Playlists
            .Include(playlist => playlist.Links)
            .FirstOrDefaultAsync(x => x.Name == command.Name);

        importedPlaylist.Should().NotBeNull();
        importedPlaylist?.Name.Should().Be(command.Name);
        importedPlaylist?.Public.Should().Be(command.Public);
        importedPlaylist?.Links.Should().HaveCount(command.ExportedLinks.Count);
        importedPlaylist?.UserId.Should().Be(user.UserId);

        foreach (var link in command.ExportedLinks)
        {
            var matchingLink = importedPlaylist?.Links.FirstOrDefault(x => x.Url == link.Url);

            matchingLink.Should().NotBeNull();
            matchingLink.Should().Match<Link>(x =>
                x.Url == link.Url &&
                x.VideoId == link.VideoId &&
                x.Title == link.Title &&
                x.Downloaded == false);
        }
    }

    [Fact]
    public async Task ImportPlaylist_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var command = new ImportPlaylist.Command
        {
            Name = "TestPlaylist",
            Public = true,
            ExportedLinks =
            [
                new LinkJsonModel
                {
                    Title = "Rick Astley - Never Gonna Give You Up (Official Music Video)",
                    Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                    VideoId = "dQw4w9WgXcQ",
                },
            ],
            PlaylistFileType = PlaylistFileType.Json,
        };

        await FluentActions.Invoking(() => PlaylistApiClient.ImportPlaylist(command))
            .Should().ThrowAsync<MyUnauthorizedException>();
    }
}