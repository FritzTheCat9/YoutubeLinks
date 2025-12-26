using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Commands;

public class CreatePlaylistFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreatePlaylist_ShouldSucceed_WhenDataIsValid()
    {
        var user = await LoginAsAdmin();

        var command = new CreatePlaylist.Command()
        {
            Name = "TestPlaylist",
            Public = true,
        };

        var playlistId = await PlaylistApiClient.CreatePlaylist(command);

        var playlist = await Context.Playlists.AsNoTracking().FirstOrDefaultAsync(x => x.Id == playlistId);

        Assert.NotNull(playlist);
        Assert.Equal(command.Name, playlist.Name);
        Assert.Equal(command.Public, playlist.Public);
        Assert.Equal(user.UserId, playlist.UserId);
    }

    [Fact]
    public async Task CreatePlaylist_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var command = new CreatePlaylist.Command()
        {
            Name = "TestPlaylist",
            Public = true,
        };

        await Logout();

        await Assert.ThrowsAsync<MyUnauthorizedException>(async () =>
        {
            await PlaylistApiClient.CreatePlaylist(command);
        });
    }
}