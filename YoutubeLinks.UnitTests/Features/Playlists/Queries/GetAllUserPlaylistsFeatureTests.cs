using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

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

        var list = new List<Playlist>
        {
            new()
            {
                Id = 1
            }
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        playlistRepository.AsQueryable(Arg.Any<int>(), Arg.Any<bool>()).Returns(list.AsQueryable());

        mediator.Send(Arg.Any<GetAllUserPlaylists.Query>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new GetAllUserPlaylistsFeature.Handler(playlistRepository, authService);
                return handler.Handle(callInfo.Arg<GetAllUserPlaylists.Query>(), CancellationToken.None);
            });

        var result = await mediator.Send(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<PagedList<PlaylistDto>>();
        result.TotalCount.Should().Be(1);
        result.Items.Count.Should().Be(1);
    }
}