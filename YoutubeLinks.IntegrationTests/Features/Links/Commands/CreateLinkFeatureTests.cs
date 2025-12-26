using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Links.Commands;

public class CreateLinkFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateLink_ShouldSucceed_WhenDataIsValid()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new CreateLink.Command()
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = playlist.Id,
        };

        var linkId = await LinkApiClient.CreateLink(command);

        var link = await Context.Links.AsNoTracking().FirstOrDefaultAsync(x => x.Id == linkId);

        Assert.NotNull(link);
        Assert.Equal(linkId, link.Id);
        Assert.Equal(command.Url, link.Url);
        Assert.Equal(command.PlaylistId, link.PlaylistId);
        Assert.False(link.Downloaded);
        Assert.Equal("dQw4w9WgXcQ", link.VideoId);
    }

    [Fact]
    public async Task CreateLink_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new CreateLink.Command()
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = playlist.Id,
        };

        await Assert.ThrowsAsync<MyUnauthorizedException>(async () =>
        {
            await LinkApiClient.CreateLink(command);
        });
    }
}