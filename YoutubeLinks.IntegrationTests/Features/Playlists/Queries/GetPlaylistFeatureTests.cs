using FluentAssertions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Playlists.Queries;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Queries;

public class GetPlaylistFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetPlaylist_ShouldReturnPlaylist()
    {
        var user = await LoginAsAdmin();

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new GetPlaylist.Query()
        {
            Id = playlist.Id
        };

        var playlistDto = await PlaylistApiClient.GetPlaylist(command.Id);

        playlistDto.Should().NotBeNull();
        playlistDto.Id.Should().Be(playlist.Id);
        playlistDto.Name.Should().Be(playlist.Name);
        playlistDto.Public.Should().Be(playlist.Public);
        playlistDto.UserId.Should().Be(playlist.UserId);
    }
}