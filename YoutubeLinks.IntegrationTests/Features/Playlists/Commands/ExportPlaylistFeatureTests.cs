using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Commands;

public class ExportPlaylistFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ExportPlaylist_ShouldSucceed_WhenDataIsValid()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new ExportPlaylist.Command
        {
            Id = playlist.Id,
            PlaylistFileType = PlaylistFileType.Json,
        };

        var response = await PlaylistApiClient.ExportPlaylist(command);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

        await using (var stream = await response.Content.ReadAsStreamAsync())
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            Assert.True(memoryStream.Length > 0);
        }

        var filename = response.Content.Headers.ContentDisposition?.FileNameStar;
        var playlistNameWithExtension =
            $"{playlist.Name}.{PlaylistHelpers.PlaylistFileTypeToString(PlaylistFileType.Json)}";

        Assert.Equal(playlistNameWithExtension, filename);
    }
}