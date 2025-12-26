using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Playlists.Queries;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Queries;

public class GetPlaylistFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetPlaylist_ShouldReturnPlaylist()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new GetPlaylist.Query()
        {
            Id = playlist.Id
        };

        var playlistDto = await PlaylistApiClient.GetPlaylist(command.Id);

        Assert.NotNull(playlistDto);
        Assert.Equal(playlist.Id, playlistDto.Id);
        Assert.Equal(playlist.Name, playlistDto.Name);
        Assert.Equal(playlist.Public, playlistDto.Public);
        Assert.Equal(playlist.UserId, playlistDto.UserId);
    }
}