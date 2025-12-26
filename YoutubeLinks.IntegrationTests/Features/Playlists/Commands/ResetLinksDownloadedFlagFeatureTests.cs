using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Commands;

public class ResetLinksDownloadedFlagFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ResetLinksDownloadedFlag_ShouldSucceed_WhenDataIsValid()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link1 = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");
        var link2 = playlist.AddLink("https://www.youtube.com/watch?v=GtUVQei3nX4", "GtUVQei3nX4", "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams");

        var links = new List<Link>()
        {
            link1,
            link2,
        };

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new ResetLinksDownloadedFlag.Command()
        {
            Id = playlist.Id,
            IsDownloaded = true,
        };

        await PlaylistApiClient.ResetLinksDownloadedFlag(command);

        var updatedPlaylist = await Context.Playlists
            .AsNoTracking()
            .Include(x => x.Links)
            .FirstOrDefaultAsync(x => x.Id == playlist.Id);

        Assert.NotNull(updatedPlaylist);
        Assert.Equal(2, updatedPlaylist.Links.Count);
        Assert.All(updatedPlaylist.Links, link => Assert.True(link.Downloaded));
    }

    [Fact]
    public async Task ResetLinksDownloadedFlag_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link1 = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");
        var link2 = playlist.AddLink("https://www.youtube.com/watch?v=GtUVQei3nX4", "GtUVQei3nX4", "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams");

        var links = new List<Link>()
        {
            link1,
            link2,
        };

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new ResetLinksDownloadedFlag.Command()
        {
            Id = playlist.Id,
            IsDownloaded = true,
        };

        await Logout();

        await Assert.ThrowsAsync<MyUnauthorizedException>(async () =>
        {
            await PlaylistApiClient.ResetLinksDownloadedFlag(command);
        });
    }
}