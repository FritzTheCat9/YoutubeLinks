using FluentAssertions;
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
        var user = await LoginAsAdmin();

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };

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
        
        updatedPlaylist.Should().NotBeNull();
        updatedPlaylist?.Name.Should().Be(command.Name);
        updatedPlaylist?.Public.Should().Be(command.Public);
    }

    [Fact]
    public async Task UpdatePlaylist_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var user = await LoginAsAdmin();

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new UpdatePlaylist.Command()
        {
            Id = playlist.Id,
            Name = "Updated Playlist",
            Public = false,
        };

        await Logout();

        await FluentActions.Invoking(() => PlaylistApiClient.UpdatePlaylist(command))
            .Should().ThrowAsync<MyUnauthorizedException>();
    }
}