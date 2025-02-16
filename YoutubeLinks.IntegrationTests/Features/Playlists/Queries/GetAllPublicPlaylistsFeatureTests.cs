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
        var adminInfo = await LoginAsAdmin();
        var admin = await GetUser(adminInfo.UserId);
        var adminPlaylist1 = Playlist.Create("Test Admin Playlist 1", false, admin);
        var adminPlaylist2 = Playlist.Create("Test Admin Playlist 2", true, admin);
        var adminPlaylist3 = Playlist.Create("Test Admin Playlist 3", false, admin);

        var userInfo = await LoginAsUser();
        var user = await GetUser(userInfo.UserId);
        var userPlaylist1 = Playlist.Create("Test User Playlist 1", true, user);
        var userPlaylist2 = Playlist.Create("Test User Playlist 2", false, user);
        var userPlaylist3 = Playlist.Create("Test User Playlist 3", true, user);

        var playlists = new List<Playlist>()
        {
            adminPlaylist1,
            adminPlaylist2,
            adminPlaylist3,
            userPlaylist1,
            userPlaylist2,
            userPlaylist3,
        };

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