using FluentAssertions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Links.Queries;

namespace YoutubeLinks.IntegrationTests.Features.Links.Queries;

public class GetAllLinksFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllLinks_ShouldReturnLinks()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link1 = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");
        var link2 = playlist.AddLink("https://www.youtube.com/watch?v=GtUVQei3nX4", "GtUVQei3nX4", "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams");

        var links = new List<Link>()
        {
            link1,
            link2
        };

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new GetAllLinks.Query
        {
            PlaylistId = playlist.Id,
            Downloaded = false,
        };

        var playlistLinks = (await LinkApiClient.GetAllLinks(command)).ToList();

        playlistLinks.Should().NotBeNull();
        playlistLinks.Should().HaveCount(2);

        foreach (var returnedLink in playlistLinks)
        {
            var matchingLink = links.FirstOrDefault(x => x.Id == returnedLink.Id);
            
            matchingLink.Should().NotBeNull();
            matchingLink.Should().Match<Link>(x =>
                x.Id == returnedLink.Id &&
                x.Url == returnedLink.Url &&
                x.VideoId == returnedLink.VideoId &&
                x.Title == returnedLink.Title);
        }
    }
}