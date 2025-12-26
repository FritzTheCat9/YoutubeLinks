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

        Assert.NotNull(importedPlaylist);
        Assert.Equal(command.Name, importedPlaylist.Name);
        Assert.Equal(command.Public, importedPlaylist.Public);
        Assert.Equal(command.ExportedLinkUrls.Count, importedPlaylist.Links.Count);
        Assert.Equal(user.UserId, importedPlaylist.UserId);

        foreach (var url in command.ExportedLinkUrls)
        {
            var matchingUrl = importedPlaylist?.Links.FirstOrDefault(x => x.Url == url);
            Assert.NotNull(matchingUrl);
            Assert.Equal(url, matchingUrl.Url);
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

        Assert.NotNull(importedPlaylist);
        Assert.Equal(command.Name, importedPlaylist.Name);
        Assert.Equal(command.Public, importedPlaylist.Public);
        Assert.Equal(command.ExportedLinks.Count, importedPlaylist.Links.Count);
        Assert.Equal(user.UserId, importedPlaylist.UserId);

        foreach (var link in command.ExportedLinks)
        {
            var matchingLink = importedPlaylist?.Links.FirstOrDefault(x => x.Url == link.Url);

            Assert.NotNull(matchingLink);
            Assert.Equal(link.Url, matchingLink.Url);
            Assert.Equal(link.VideoId, matchingLink.VideoId);
            Assert.Equal(link.Title, matchingLink.Title);
            Assert.False(matchingLink.Downloaded);
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

        await Assert.ThrowsAsync<MyUnauthorizedException>(async () =>
        {
            await PlaylistApiClient.ImportPlaylist(command);
        });
    }
}