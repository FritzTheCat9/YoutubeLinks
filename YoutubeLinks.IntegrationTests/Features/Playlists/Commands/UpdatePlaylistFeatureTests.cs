using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Commands;

public class UpdatePlaylistFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdatePlaylist_ShouldSucceed_WhenDataIsValid()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new UpdatePlaylist.Command()
        {
            Id = playlist.Id,
            Name = "Updated Playlist",
            Public = false,
        };

        await PlaylistApiClient.UpdatePlaylist(command);

        var updatedPlaylist = await Context.Playlists
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == playlist.Id);

        Assert.NotNull(updatedPlaylist);
        Assert.Equal(command.Name, updatedPlaylist.Name);
        Assert.Equal(command.Public, updatedPlaylist.Public);
    }

    [Fact]
    public async Task UpdatePlaylist_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new UpdatePlaylist.Command()
        {
            Id = playlist.Id,
            Name = "Updated Playlist",
            Public = false,
        };

        await Logout();

        await Assert.ThrowsAsync<MyUnauthorizedException>(async () =>
        {
            await PlaylistApiClient.UpdatePlaylist(command);
        });
    }
}