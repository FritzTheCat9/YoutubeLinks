using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

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

        var mediator = Substitute.For<IMediator>();

        _playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        var handler = new GetAllPaginatedLinksFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => mediator.Send(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllPaginatedLinksHandler_ReturnsLinksPagedList()
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

        var list = new List<Link>
        {
            new()
            {
                Id = 1
            }
        };

        _playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
        {
            UserId = 1
        });
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        _playlistRepository.GetPlaylistLinksAsQueryable(Arg.Any<int>(), Arg.Any<bool>()).Returns(list.AsQueryable());

        var handler = new GetAllPaginatedLinksFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PagedList<LinkDto>>(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
    }
}