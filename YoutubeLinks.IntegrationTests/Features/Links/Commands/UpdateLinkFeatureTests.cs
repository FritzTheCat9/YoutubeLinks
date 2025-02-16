using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Links.Commands;

public class UpdateLinkFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateLink_ShouldSucceed_WhenDataIsValid()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new UpdateLink.Command()
        {
            Id = link.Id,
            Url = link.Url,
            Title = "Rick Astley - Never Gonna Give You Up",
            Downloaded = true,
        };

        await LinkApiClient.UpdateLink(command);

        var updatedLink = await Context.Links.AsNoTracking().FirstOrDefaultAsync(x => x.Id == link.Id);
        updatedLink?.Title.Should().Be(command.Title);
        updatedLink?.Downloaded.Should().Be(command.Downloaded);
    }

    [Fact]
    public async Task UpdateLink_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new UpdateLink.Command()
        {
            Id = link.Id,
            Url = link.Url,
            Title = "Rick Astley - Never Gonna Give You Up",
            Downloaded = true,
        };

        await FluentActions.Invoking(() => LinkApiClient.UpdateLink(command))
            .Should().ThrowAsync<MyUnauthorizedException>();
    }
}