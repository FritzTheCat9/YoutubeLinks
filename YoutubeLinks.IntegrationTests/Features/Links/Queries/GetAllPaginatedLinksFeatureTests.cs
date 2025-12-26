using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Links.Queries;

namespace YoutubeLinks.IntegrationTests.Features.Links.Queries;

public class GetAllPaginatedLinksFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllPaginatedLinks_ShouldReturnPaginatedLinks()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link1 = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");
        var link2 = playlist.AddLink("https://www.youtube.com/watch?v=GtUVQei3nX4", "GtUVQei3nX4", "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams");
        var link3 = playlist.AddLink("https://www.youtube.com/watch?v=u15tEo0wsQI", "u15tEo0wsQI", "Dawid Podsiadło, P.T. Adamczyk — Phantom Liberty (Official Cyberpunk 2077 Music Video)");

        var links = new List<Link>()
        {
            link1,
            link2,
            link3
        };

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new GetAllPaginatedLinks.Query
        {
            SearchTerm = "",
            PlaylistId = playlist.Id,
            Page = 1,
            PageSize = 2,
            SortColumn = "title",
            SortOrder = SortOrder.Ascending,
        };

        var paginatedLinks = await LinkApiClient.GetAllPaginatedLinks(command);

        Assert.NotNull(paginatedLinks);
        Assert.Equal(2, paginatedLinks.Items.Count);
        var sorted = paginatedLinks.Items.OrderBy(x => x.Title).ToList();
        Assert.Equal(sorted, paginatedLinks.Items);
        Assert.Equal(command.Page, paginatedLinks.Page);
        Assert.Equal(command.PageSize, paginatedLinks.PageSize);
        Assert.Equal(3, paginatedLinks.TotalCount);
        Assert.Equal(2, paginatedLinks.PagesCount);
        Assert.True(paginatedLinks.HasNextPage);
        Assert.False(paginatedLinks.HasPreviousPage);

        foreach (var returnedLink in paginatedLinks.Items)
        {
            var matchingLink = links.FirstOrDefault(x => x.Id == returnedLink.Id);

            Assert.NotNull(matchingLink);
            Assert.Equal(returnedLink.Id, matchingLink.Id);
            Assert.Equal(returnedLink.Url, matchingLink.Url);
            Assert.Equal(returnedLink.VideoId, matchingLink.VideoId);
            Assert.Equal(returnedLink.Title, matchingLink.Title);
            Assert.Equal(returnedLink.Downloaded, matchingLink.Downloaded);
            Assert.Equal(returnedLink.PlaylistId, matchingLink.PlaylistId);
        }
    }
}