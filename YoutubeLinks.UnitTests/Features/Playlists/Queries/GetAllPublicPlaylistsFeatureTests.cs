using NSubstitute;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

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

        var list = new List<Playlist>
        {
            new()
            {
                Id = 1
            }
        };

        _playlistRepository.GetAllPublic().Returns(list.AsQueryable());

        var handler = new GetAllPublicPlaylistsFeature.Handler(_playlistRepository);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PagedList<PlaylistDto>>(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
    }
}