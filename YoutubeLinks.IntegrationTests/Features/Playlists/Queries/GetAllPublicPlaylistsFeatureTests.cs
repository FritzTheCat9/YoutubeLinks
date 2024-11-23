using FluentAssertions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Queries;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Queries;

public class GetAllPublicPlaylistsFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllPublicPlaylists_ShouldReturnPaginatedPublicPlaylists()
    {
        var admin = await LoginAsAdmin();

        var adminPlaylists = new List<Playlist>()
        {
            new() { Name = "Test Admin Playlist 1", Public = false, UserId = admin.UserId },
            new() { Name = "Test Admin Playlist 2", Public = true, UserId = admin.UserId },
            new() { Name = "Test Admin Playlist 3", Public = false, UserId = admin.UserId }
        };

        var user = await LoginAsUser();

        var userPlaylists = new List<Playlist>()
        {
            new() { Name = "Test User Playlist 1", Public = true, UserId = user.UserId },
            new() { Name = "Test User Playlist 2", Public = false, UserId = user.UserId },
            new() { Name = "Test User Playlist 3", Public = true, UserId = user.UserId }
        };

        var playlists = new List<Playlist>();
        playlists.AddRange(adminPlaylists);
        playlists.AddRange(userPlaylists);

        await Context.Playlists.AddRangeAsync(playlists);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new GetAllPublicPlaylists.Query
        {
            SearchTerm = "",
            Page = 1,
            PageSize = 2,
            SortColumn = "name",
            SortOrder = SortOrder.Ascending,
        };

        var paginatedPublicPlaylists = await PlaylistApiClient.GetAllPublicPlaylists(command);

        paginatedPublicPlaylists.Should().NotBeNull();
        paginatedPublicPlaylists.Items.Should().HaveCount(2);
        paginatedPublicPlaylists.Items.Should().BeInAscendingOrder(x => x.Name);
        paginatedPublicPlaylists.Page.Should().Be(command.Page);
        paginatedPublicPlaylists.PageSize.Should().Be(command.PageSize);
        paginatedPublicPlaylists.TotalCount.Should().Be(3);
        paginatedPublicPlaylists.PagesCount.Should().Be(2);
        paginatedPublicPlaylists.HasNextPage.Should().Be(true);
        paginatedPublicPlaylists.HasPreviousPage.Should().Be(false);

        foreach (var returnedPlaylist in paginatedPublicPlaylists.Items)
        {
            var matchingPlaylist = playlists.FirstOrDefault(x => x.Id == returnedPlaylist.Id);

            matchingPlaylist.Should().NotBeNull();
            matchingPlaylist.Should().Match<Playlist>(x =>
                x.Id == returnedPlaylist.Id &&
                x.Name == returnedPlaylist.Name &&
                x.Public == returnedPlaylist.Public &&
                x.UserId == returnedPlaylist.UserId);
        }
    }
}