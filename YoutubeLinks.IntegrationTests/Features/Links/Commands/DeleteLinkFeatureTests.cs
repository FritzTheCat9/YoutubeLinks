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
        var user = await LoginAsAdmin();

        var link = new Link
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            VideoId = "dQw4w9WgXcQ",
            Title = "Rick Astley - Never Gonna Give You Up (Official Music Video)",
            Downloaded = false,
        };

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        playlist.Links.Add(link);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new DeleteLink.Command()
        {
            Id = link.Id,
        };

        await LinkApiClient.DeleteLink(command.Id);
        
        var deletedLink = await Context.Links.FirstOrDefaultAsync(x => x.Id == link.Id);
        deletedLink.Should().BeNull();
    }

    [Fact]
    public async Task DeleteLink_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var user = await LoginAsAdmin();

        var link = new Link
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            VideoId = "dQw4w9WgXcQ",
            Title = "Rick Astley - Never Gonna Give You Up (Official Music Video)",
            Downloaded = false,
        };

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        playlist.Links.Add(link);

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