using NSubstitute;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;
using YoutubeLinks.UnitTests.Builders;

namespace YoutubeLinks.UnitTests.Features.Playlists.Queries;

public class GetAllPublicPlaylistsFeatureTests
{
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task GetAllPublicPlaylistsHandler_ReturnsPlaylistsPagedList()
    {
        var query = new GetAllPublicPlaylists.Query
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "name",
            SortOrder = SortOrder.Ascending,
            SearchTerm = ""
        };

        var publicPlaylist = PlaylistBuilder.Create()
            .Public(true)
            .Build();

        var playlists = new List<Playlist> { publicPlaylist };
        var pagedPlaylists = new PagedList<Playlist>(
            items: playlists,
            page: query.Page,
            pageSize: query.PageSize,
            totalCount: playlists.Count
        );

        _playlistRepository.GetAllPublicPlaylistsPaginated(query).Returns(pagedPlaylists);

        var handler = new GetAllPublicPlaylistsFeature.Handler(_playlistRepository);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PagedList<PlaylistDto>>(result);
        Assert.Equal(pagedPlaylists.TotalCount, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(publicPlaylist.Id, result.Items.First().Id);
    }
}
