using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Links.Commands;

public class DeleteLinkFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteLink_ShouldSucceed_WhenDataIsValid()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new DeleteLink.Command()
        {
            Id = link.Id,
        };

        await LinkApiClient.DeleteLink(command.Id);

        var deletedLink = await Context.Links.AsNoTracking().FirstOrDefaultAsync(x => x.Id == link.Id);
        deletedLink.Should().BeNull();
    }

    [Fact]
    public async Task DeleteLink_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new DeleteLink.Command()
        {
            Id = link.Id,
        };

        await FluentActions.Invoking(() => LinkApiClient.DeleteLink(command.Id))
            .Should().ThrowAsync<MyUnauthorizedException>();
    }
}