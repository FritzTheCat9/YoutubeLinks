using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.IntegrationTests.Features.Links.Queries;

public class GetLinkFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetLink_ShouldReturnLink()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new GetLink.Query()
        {
            Id = link.Id,
        };

        var linkDto = await LinkApiClient.GetLink(command.Id);

        Assert.NotNull(linkDto);
        Assert.Equal(link.Id, linkDto.Id);
        Assert.Equal(link.Url, linkDto.Url);
        Assert.Equal(link.VideoId, linkDto.VideoId);
        Assert.Equal(link.Title, linkDto.Title);
        Assert.Equal(link.Downloaded, linkDto.Downloaded);
        Assert.Equal(link.PlaylistId, linkDto.PlaylistId);
    }
}