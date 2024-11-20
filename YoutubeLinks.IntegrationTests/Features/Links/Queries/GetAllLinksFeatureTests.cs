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
        var user = await LoginAsAdmin();

        var links = new List<Link>
        {
            new()
            {
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                VideoId = "dQw4w9WgXcQ",
                Title = "Rick Astley - Never Gonna Give You Up (Official Music Video)",
                Downloaded = false,
            },
            new()
            {
                Url = "https://www.youtube.com/watch?v=GtUVQei3nX4",
                VideoId = "GtUVQei3nX4",
                Title = "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams",
                Downloaded = false,
            }
        };

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        playlist.Links.AddRange(links);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new GetAllLinks.Query
        {
            PlaylistId = playlist.Id,
            Downloaded = false,
        };

        var playlistLinks = (await LinkApiClient.GetAllLinks(command)).ToList();

        playlistLinks.Should().NotBeNull();
        playlistLinks.Count().Should().Be(2);

        foreach (var returnedLink in playlistLinks)
        {
            var matchingLink = links.FirstOrDefault(x => x.Id == returnedLink.Id);
            matchingLink.Should().NotBeNull();
            matchingLink?.Id.Should().Be(returnedLink.Id);
            matchingLink?.Url.Should().Be(returnedLink.Url);
            matchingLink?.VideoId.Should().Be(returnedLink.VideoId);
            matchingLink?.Title.Should().Be(returnedLink.Title);
        }
    }
}