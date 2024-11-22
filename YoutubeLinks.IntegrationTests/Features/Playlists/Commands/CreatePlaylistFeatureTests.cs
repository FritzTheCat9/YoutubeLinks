using FluentAssertions;
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

        playlist.Should().NotBeNull();
        playlist?.Name.Should().Be(command.Name);
        playlist?.Public.Should().Be(command.Public);
        playlist?.UserId.Should().Be(user.UserId);
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

        await FluentActions.Invoking(() => PlaylistApiClient.CreatePlaylist(command))
            .Should().ThrowAsync<MyUnauthorizedException>();
    }
}