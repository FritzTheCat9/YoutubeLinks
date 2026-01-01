using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;
using YoutubeLinks.UnitTests.Builders;

namespace YoutubeLinks.UnitTests.Features.Links.Queries;

public class GetAllPaginatedLinksFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task GetAllPaginatedLinksHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var query = new GetAllPaginatedLinks.Query
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "title",
            SortOrder = SortOrder.Ascending,
            SearchTerm = "",
            PlaylistId = 1
        };

        _playlistRepository.Get(Arg.Any<int>()).Returns((Playlist)null);

        var handler = new GetAllPaginatedLinksFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllPaginatedLinksHandler_ReturnsLinksPagedList_ForOwner()
    {
        var query = new GetAllPaginatedLinks.Query
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "title",
            SortOrder = SortOrder.Ascending,
            SearchTerm = "",
            PlaylistId = 1
        };

        var user = UserBuilder.Create().WithEmail("owner@mail.com").Build();
        var playlist = PlaylistBuilder.Create()
            .WithUser(user)
            .WithLink("https://youtu.be/abc", "abc", "Test Video")
            .Build();

        _playlistRepository.Get(query.PlaylistId).Returns(playlist);
        _authService.IsLoggedInUser(playlist.UserId).Returns(true);
        _playlistRepository.GetPlaylistLinksAsQueryable(query.PlaylistId, true)
            .Returns(playlist.Links.AsQueryable());

        var handler = new GetAllPaginatedLinksFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PagedList<LinkDto>>(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Test Video", result.Items.First().Title);
    }

    [Fact]
    public async Task GetAllPaginatedLinksHandler_ReturnsLinksPagedList_ForPublicPlaylist_WhenUserNotOwner()
    {
        var query = new GetAllPaginatedLinks.Query
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "title",
            SortOrder = SortOrder.Ascending,
            SearchTerm = "",
            PlaylistId = 1
        };

        var user = UserBuilder.Create().WithEmail("owner@mail.com").Build();
        var playlist = PlaylistBuilder.Create()
            .WithUser(user)
            .Public(true)
            .WithLink("https://youtu.be/xyz", "xyz", "Public Video")
            .Build();

        _playlistRepository.Get(query.PlaylistId).Returns(playlist);
        _authService.IsLoggedInUser(playlist.UserId).Returns(false);
        _playlistRepository.GetPlaylistLinksAsQueryable(query.PlaylistId, false)
            .Returns(playlist.Links.AsQueryable());

        var handler = new GetAllPaginatedLinksFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PagedList<LinkDto>>(result);
        Assert.Single(result.Items);
        Assert.Equal("Public Video", result.Items.First().Title);
    }
}
