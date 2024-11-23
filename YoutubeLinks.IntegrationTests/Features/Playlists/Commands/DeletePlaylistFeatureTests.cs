using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Commands;

public class DeletePlaylistFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeletePlaylist_ShouldSucceed_WhenDataIsValid()
    {
        var user = await LoginAsAdmin();

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new DeletePlaylist.Command()
        {
            Id = playlist.Id
        };

        await PlaylistApiClient.DeletePlaylist(command.Id);

        var deletedPlaylist = await Context.Playlists.AsNoTracking().FirstOrDefaultAsync(x => x.Id == playlist.Id);
        deletedPlaylist.Should().BeNull();
    }

    [Fact]
    public async Task DeletePlaylist_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var user = await LoginAsAdmin();

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new DeletePlaylist.Command()
        {
            Id = playlist.Id
        };

        await Logout();

        await FluentActions.Invoking(() => PlaylistApiClient.DeletePlaylist(command.Id))
            .Should().ThrowAsync<MyUnauthorizedException>();
    }
}