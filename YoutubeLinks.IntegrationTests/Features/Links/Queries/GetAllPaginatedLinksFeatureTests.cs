using FluentAssertions;
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
            },
            new()
            {
                Url = "https://www.youtube.com/watch?v=u15tEo0wsQI",
                VideoId = "u15tEo0wsQI",
                Title = "Dawid Podsiadło, P.T. Adamczyk — Phantom Liberty (Official Cyberpunk 2077 Music Video)",
                Downloaded = false,
            }
        };

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        playlist.Links.AddRange(links);

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

        paginatedLinks.Should().NotBeNull();
        paginatedLinks.Items.Should().HaveCount(2);
        paginatedLinks.Items.Should().BeInAscendingOrder(x => x.Title);
        paginatedLinks.Page.Should().Be(command.Page);
        paginatedLinks.PageSize.Should().Be(command.PageSize);
        paginatedLinks.TotalCount.Should().Be(3);
        paginatedLinks.PagesCount.Should().Be(2);
        paginatedLinks.HasNextPage.Should().Be(true);
        paginatedLinks.HasPreviousPage.Should().Be(false);
        
        foreach (var returnedLink in paginatedLinks.Items)
        {
            var matchingLink = links.FirstOrDefault(x => x.Id == returnedLink.Id);
            matchingLink.Should().NotBeNull();

            matchingLink!.Should().Match<Link>(x =>
                x.Id == returnedLink.Id &&
                x.Url == returnedLink.Url &&
                x.VideoId == returnedLink.VideoId &&
                x.Title == returnedLink.Title &&
                x.Downloaded == returnedLink.Downloaded &&
                x.PlaylistId == returnedLink.PlaylistId);
        }
    }
}