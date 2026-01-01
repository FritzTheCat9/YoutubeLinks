using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;
using YoutubeLinks.UnitTests.Builders;

namespace YoutubeLinks.UnitTests.Features.Playlists.Queries;

public class GetAllUserPlaylistsFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task GetAllUserPlaylistsHandler_ReturnsPlaylistsPagedList()
    {
        var query = new GetAllUserPlaylists.Query
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "name",
            SortOrder = SortOrder.Ascending,
            SearchTerm = "",
            UserId = 1
        };

        var playlist = new PlaylistBuilder().WithName("Playlist 1").Build();
        var playlists = new List<Playlist> { playlist };

        var pagedList = new PagedList<Playlist>(
            playlists,
            query.Page,
            query.PageSize,
            playlists.Count
        );

        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        _playlistRepository
            .GetAllUserPlaylistsPaginated(
                Arg.Any<GetAllUserPlaylists.Query>(),
                Arg.Any<int>(),
                Arg.Any<bool>()
            )
            .Returns(pagedList);

        var handler = new GetAllUserPlaylistsFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PagedList<PlaylistDto>>(result);
        Assert.Equal(pagedList.TotalCount, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Playlist 1", result.Items.First().Name);
    }
}
