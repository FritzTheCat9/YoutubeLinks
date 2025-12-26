using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Links.Commands;

public class SetLinkDownloadedFlagFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task SetLinkDownloadedFlag_ShouldSucceed_WhenDataIsValid()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new SetLinkDownloadedFlag.Command()
        {
            Id = link.Id,
            Downloaded = true,
        };

        await LinkApiClient.SetLinkDownloadedFlag(command);

        var updatedLink = await Context.Links.AsNoTracking().FirstOrDefaultAsync(x => x.Id == link.Id);
        Assert.NotNull(updatedLink);
        Assert.Equal(command.Downloaded, updatedLink.Downloaded);
    }

    [Fact]
    public async Task SetLinkDownloadedFlag_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new SetLinkDownloadedFlag.Command()
        {
            Id = link.Id,
            Downloaded = true,
        };

        await Assert.ThrowsAsync<MyUnauthorizedException>(async () =>
        {
            await LinkApiClient.SetLinkDownloadedFlag(command);
        });
    }
}